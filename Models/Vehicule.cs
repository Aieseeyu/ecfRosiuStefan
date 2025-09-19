using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("VEHICULE")]
    public class Vehicule
    {

        [StringLength(9)]
        [Column("n°immatriculation")]
        public string Immatriculation { get; set; }

        [Required]
        [StringLength(50)]
        [Column("modèle véhicule")]
        public string ModeleVehicule { get; set; }

        [Required]
        [Column("état")] 
        public bool Etat { get; set; }

        public Vehicule() { }

        public Vehicule(string immatriculation, string modeleVehicule, bool etat)
        {
            Immatriculation = immatriculation;
            ModeleVehicule = modeleVehicule;
            Etat = etat;
        }
    }
}
