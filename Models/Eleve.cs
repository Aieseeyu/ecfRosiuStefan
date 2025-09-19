using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("ELEVE")]
    public class Eleve
    {
        [Column("id élève")]
        public int IdEleve { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nom élève")]
        public string NomEleve { get; set; }

        [Required]
        [StringLength(50)]
        [Column("prénom élève")]
        public string PrenomEleve { get; set; }

        [Required]
        [Column("code")]
        public bool Code { get; set; }

        [Required]
        [Column("conduite")]
        public bool Conduite { get; set; }

        [Required]
        [Column("date naissance", TypeName = "date")]
        public DateTime DateNaissance { get; set; }
 
        public Eleve() { }

        public Eleve(int idEleve, string nomEleve, string prenomEleve, bool code, bool conduite, DateTime dateNaissance)
        {
            IdEleve = idEleve;
            NomEleve = nomEleve;
            PrenomEleve = prenomEleve;
            Code = code;
            Conduite = conduite;
            DateNaissance = dateNaissance.Date;
        }
    }
}
