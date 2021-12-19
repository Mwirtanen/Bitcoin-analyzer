using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace Bitcoin_analyzer
{
    class App : GUI
    {
        
        public static void Main (string[] args)
        {
            
            GUI display = new GUI();
            display.DisplayGUI();

        }

        
    }
}
