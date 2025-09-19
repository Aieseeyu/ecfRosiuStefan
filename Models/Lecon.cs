using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("LECON")]
    public class Lecon
    {
        [StringLength(50)]
        [Column("modèle véhicule")]
        public string ModeleVehicule { get; set; }

        [Column("date heure", TypeName = "date")]
        public DateTime DateHeure { get; set; }

        [Column("id élève")]
        public int IdEleve { get; set; }

        [Column("id moniteur")]
        public int IdMoniteur { get; set; }

        [Required]
        [Column("durée")]
        public int Duree { get; set; }
        public Lecon() { }
        public Lecon(string modeleVehicule, DateTime dateHeure, int idEleve, int idMoniteur, int duree)
        {
            ModeleVehicule = modeleVehicule;
            DateHeure = dateHeure.Date;
            IdEleve = idEleve;
            IdMoniteur = idMoniteur;
            Duree = duree;
        }
    }
}