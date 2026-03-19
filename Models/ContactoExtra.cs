namespace SysMarkModerno.Models
{
    public class ContactoExtra
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        
        public bool IsSelected { get; set; }
        
        public override string ToString()
        {
            return Nombre;
        }
    }
}
