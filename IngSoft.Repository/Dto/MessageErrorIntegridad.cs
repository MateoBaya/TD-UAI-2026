namespace IngSoft.Repository.Dto
{
    public class MessageErrorIntegridad
    {
        public string Tabla { get; set; }
        public string Id { get; set; }
        public string DVEsperado { get; set; }
        public string DVCalculado { get; set; }
        public string TipoDV { get; set; }
    }
}
