using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("MODELE")]
    public class Modele
    {
        [StringLength(50)]
        [Column("modèle véhicule")]
        public string ModeleVehicule { get; set; }

        [Required]
        [StringLength(50)]
        [Column("marque")]
        public string Marque { get; set; }

        [Required]
        [StringLength(4)]
        [Column("année")]
        public string Annee { get; set; }

        [Required]
        [Column("date achat", TypeName = "date")]
        public DateTime DateAchat { get; set; }

        public Modele() { }

        public Modele(string modeleVehicule, string marque, string annee, DateTime dateAchat)
        {
            ModeleVehicule = modeleVehicule;
            Marque = marque;
            Annee = annee;
            DateAchat = dateAchat.Date;
        }
    }
}
