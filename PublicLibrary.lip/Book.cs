using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicLibrary.lip
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Edition { get; set; }

        public DateTime IssueDate { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool IsAvailible { get; set; }

        public bool IsEighteenPlus { get; set; }

        public bool IsRaritet { get; set; }

        public bool IsTheLastestPublisher { get; set; }

        public int AddedBy { get; set; }

        public DateTime AddedTime { get; set; }



    }
}
