using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECFautoecole.Models
{
    [Table("CALENDRIER")]
    public class Calendrier
    {
        [Column("date heure", TypeName = "date")]
        public DateTime DateHeure { get; set; }

        public Calendrier() { }

        public Calendrier(DateTime dateHeure)
        {
            DateHeure = dateHeure.Date;
        }
    }
}
