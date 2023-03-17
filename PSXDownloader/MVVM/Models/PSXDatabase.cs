using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PSXDownloader.MVVM.Models
{
    public class PSXDatabase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? TitleID { get; set; }
        public string? LocalPath { get; set; }
    }
}
