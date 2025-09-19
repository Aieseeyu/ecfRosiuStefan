using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("MONITEUR")]
    public class Moniteur
    {
        [Column("id moniteur")]
        public int IdMoniteur { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nom moniteur")]
        public string NomMoniteur { get; set; }

        [Required]
        [StringLength(50)]
        [Column("prénom moniteur")]
        public string PrenomMoniteur { get; set; }

        [Required]
        [Column("date naissance", TypeName = "date")]
        public DateTime DateNaissance { get; set; }

        [Required]
        [Column("date embauche", TypeName = "date")]
        public DateTime DateEmbauche { get; set; }

        [Required]
        [Column("activité")]
        public bool Activite { get; set; }

        public Moniteur() { }

        public Moniteur(int idMoniteur, string nomMoniteur, string prenomMoniteur, DateTime dateNaissance, DateTime dateEmbauche, bool activite)
        {
            IdMoniteur = idMoniteur;
            NomMoniteur = nomMoniteur;
            PrenomMoniteur = prenomMoniteur;
            DateNaissance = dateNaissance.Date;
            DateEmbauche = dateEmbauche.Date;
            Activite = activite;
        }
    }
}
