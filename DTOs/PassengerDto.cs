using System.ComponentModel.DataAnnotations;

namespace AcmeAirlines.DTOs
{
    public class PassengerDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener m�s de 100 caracteres")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede tener m�s de 100 caracteres")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El n�mero de documento es obligatorio")]
        [StringLength(20, ErrorMessage = "El n�mero de documento no puede tener m�s de 20 caracteres")]
        [Display(Name = "N�mero de documento")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        [Display(Name = "Tipo de documento")]
        public string DocumentType { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "El correo electr�nico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electr�nico v�lido")]
        [Display(Name = "Correo electr�nico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El n�mero de tel�fono es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un n�mero de tel�fono v�lido")]
        [Display(Name = "Tel�fono")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "La nacionalidad es obligatoria")]
        [Display(Name = "Nacionalidad")]
        public string Nationality { get; set; }

        // Campo para verificar si es un menor que viaja solo
        [Display(Name = "�Es un menor que viaja solo?")]
        public bool IsUnaccompaniedMinor { get; set; }

        // Propiedades calculadas
        public bool IsMinor => DateTime.Today.AddYears(-18) < DateOfBirth;

        public int Age => DateTime.Today.Year - DateOfBirth.Year -
                         (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    }

    public class PassengerListDto
    {
        public PassengerDto MainPassenger { get; set; }

        public List<PassengerDto> AdditionalPassengers { get; set; } = new List<PassengerDto>();

        public ContactInfoDto EmergencyContact { get; set; }

        public int TotalPassengers => 1 + AdditionalPassengers.Count;
    }

    public class ContactInfoDto
    {
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Parentesco/Relaci�n")]
        public string Relationship { get; set; }

        [Phone(ErrorMessage = "Ingrese un n�mero de tel�fono v�lido")]
        [Display(Name = "Tel�fono")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo electr�nico v�lido")]
        [Display(Name = "Correo electr�nico")]
        public string Email { get; set; }
    }
}