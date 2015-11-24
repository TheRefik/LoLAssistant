using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.SystemClass
{
    class DataTableSort
    {
        public DataTable DataTableSortMethod(DataTable DataSource,string TextBoxString)
        {
            DataSource.DefaultView.RowFilter = string.Format("Champion LIKE '%{0}%'",TextBoxString);
            return DataSource;
        }
    }
}
