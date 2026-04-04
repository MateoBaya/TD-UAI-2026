namespace IngSoft.Abstractions.Multidioma
{
    public interface IIdioma: IEntity, ISubject
    {
        string Nombre { get; set; }
        string Codigo { get; set; }
        bool isDefault { get; set; }
    }
}
