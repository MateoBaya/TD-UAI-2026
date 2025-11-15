using IngSoft.DBConnection.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace IngSoft.DBConnection
{
    public class SqlServerConnection : IConnection
    {
        private SqlConnection _sqlConnection;
        private SqlTransaction _oneTransaction;

        public void NuevaConexion(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
        }
        public void FinalizarConexion()
        {
            if (_sqlConnection.State != System.Data.ConnectionState.Closed)
            {
                _sqlConnection.Close();
            }
        }
        public void IniciarTransaccion()
        {
            _oneTransaction = _sqlConnection.BeginTransaction();
        }
        public void AceptarTransaccion()
        {
            _oneTransaction.Commit();
        }

        public void CancelarTransaccion()
        {
            _oneTransaction.Rollback();
        }
        public List<T> EjecutarDataSet<T>(string storeProcedure, Dictionary<string, object> parametros) where T : new()
        {
            var finalResult = new List<T>();

            using (var oneCommand = new SqlCommand(storeProcedure, _sqlConnection))
            {
                oneCommand.CommandType = CommandType.StoredProcedure;

                if (_oneTransaction != null)
                {
                    oneCommand.Transaction = _oneTransaction;
                }

                if (parametros != null)
                {
                    foreach (var p in parametros)
                    {
                        oneCommand.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                    }
                }

                using (var adapter = new SqlDataAdapter(oneCommand))
                {
                    var ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds.Tables.Count == 0) return finalResult;

                    var table = ds.Tables[0];
                    foreach (DataRow row in table.Rows)
                    {
                        var inst = new T();
                        MapDataRowToInstance(row, inst);
                        finalResult.Add(inst);
                    }
                }
            }

            return finalResult;
        }

        public List<T> EjecutarDataTable<T>(string query, Dictionary<string, object> parametros) where T : new()
        {
            List<T> finalResult = new List<T>();
            SqlCommand oneCommand = new SqlCommand();

            oneCommand.Connection = _sqlConnection;
            oneCommand.Transaction = _oneTransaction;
            oneCommand.CommandText = query;

            foreach (var p in parametros)
            {
                SqlParameter param = new SqlParameter();
                oneCommand.Parameters.AddWithValue(p.Key, p.Value);
            }

            oneCommand.CommandType = CommandType.Text;

            SqlDataReader resultDataReader = oneCommand.ExecuteReader();

            while (resultDataReader.Read())
            {
                T resultObject = new T();

                for (int i = 0; i <= resultDataReader.FieldCount - 1; i++)
                {
                    string columnName = resultDataReader.GetName(i);
                    object columnValue = resultDataReader[i];
                    PropertyInfo pi = null;
                    if (!(resultObject is DBNull))
                    {
                        pi = resultObject.GetType().GetProperty(columnName);
                    }
                    if (pi != null && !(columnValue is DBNull))
                    {
                        if(pi.PropertyType.Name == "Guid")
                        {
                            try
                            {
                                Guid guid;
                                if(Guid.TryParse(columnValue.ToString(), out guid))
                                {
                                    pi.SetValue(resultObject, guid);
                                }
                                else
                                {
                                    byte[] bytes = new byte[16];
                                    BitConverter.GetBytes((int)columnValue).CopyTo(bytes, 0);
                                    pi.SetValue(resultObject, new Guid(bytes));
                                    //if ((typeof(UsuarioQuerySql)).IsSubclassOf(resultObject.GetType()))
                                    if(resultObject.GetType().Equals(typeof(UsuarioQuerySql)))
                                    {
                                        pi = resultObject.GetType().GetProperty("IdUsuario");
                                        pi.SetValue(resultObject, columnValue);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                        else
                        {
                            pi.SetValue(resultObject, columnValue);
                        }
                    }
                }

                finalResult.Add(resultObject);

            }
            resultDataReader.Close();
            return finalResult;
        }

        public object EjecutarEscalar(string query, Dictionary<string, object> parametros)
        {
            SqlCommand oneCommand = new SqlCommand();
            oneCommand.Connection = _sqlConnection;
            oneCommand.Transaction = _oneTransaction;

            oneCommand.CommandText = query;

            foreach (var p in parametros)
            {
                oneCommand.Parameters.AddWithValue(p.Key, p.Value);
            }
            oneCommand.CommandType = CommandType.Text;
            return oneCommand.ExecuteScalar();
        }
        public int ObtenerUltimoId(string tabla, string columnaId)
        {
            string query = $"SELECT MAX({columnaId}) FROM {tabla}";
            int id = 0;
            var parametros = new Dictionary<string, object> { };
            var ob=EjecutarEscalar(query, parametros);
            if (ob != DBNull.Value && ob != null)
            {
                id = (int)(ob);
            }
            return id;
        }
        public void EjecutarSinResultado(string storeProcedure, Dictionary<string, object> parametros)
        {
            using (var oneCommand = new SqlCommand(storeProcedure, _sqlConnection))
            {
                oneCommand.CommandType = CommandType.StoredProcedure;

                if (_oneTransaction != null)
                {
                    oneCommand.Transaction = _oneTransaction;
                }

                foreach (var p in parametros)
                {
                    oneCommand.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
                }

                oneCommand.ExecuteNonQuery();
            }
            }

        private void MapDataRowToInstance<T>(DataRow row, T instance)
        {
            var t = typeof(T);
            foreach (DataColumn col in row.Table.Columns)
            {
                object value = row[col];
                if (value == null || value == DBNull.Value) continue;

                var pi = t.GetProperty(col.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi == null || !pi.CanWrite) continue;

                try
                {
                    object converted = ConvertValue(value, pi.PropertyType, instance, pi);
                    if (converted != null)
                    {
                        pi.SetValue(instance, converted);
                    }
                }
                catch
                {
                }
            }
        }

        private object ConvertValue(object value, Type targetType, object instance, PropertyInfo pi)
        {
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlying == typeof(Guid))
                return ConvertToGuid(value, instance);

            if (underlying.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(underlying, value.ToString(), true);
                return Enum.ToObject(underlying, value);
            }

            if (underlying == typeof(string))
                return value.ToString();

            if (underlying.IsAssignableFrom(value.GetType()))
                return value;

            return Convert.ChangeType(value, underlying);
        }

        private object ConvertToGuid(object value, object instance)
        {
            if (value is Guid g) return g;

            if (value is string s)
            {
                Guid parsed;
                if (Guid.TryParse(s, out parsed)) return parsed;
            }

            if (value is byte[] bytes && bytes.Length == 16)
                return new Guid(bytes);

            if (value is int intVal)
            {
                var buffer = new byte[16];
                BitConverter.GetBytes(intVal).CopyTo(buffer, 0);
                var guidGen = new Guid(buffer);

                if (instance.GetType().Equals(typeof(UsuarioQuerySql)))
                {
                    var idUsuarioPi = instance.GetType().GetProperty("IdUsuario", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (idUsuarioPi != null && idUsuarioPi.CanWrite)
                        idUsuarioPi.SetValue(instance, intVal);
                }
                return guidGen;
            }

            Guid fallback;
            if (Guid.TryParse(value.ToString(), out fallback))
                return fallback;

            return null;
        }
    }
}
