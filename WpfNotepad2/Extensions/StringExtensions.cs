using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotepad2.Extensions
{
    public static class StringExtensions
    {
        public static string ToUriPath(this string simplePath)
        {
            return "pack://application:,,,/" + simplePath;;
        }
    }
}
