using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS_2_zadanie_KLIENT
{
    class Fragment
    {
        public Fragment(int sizeOfFragment)
        {
            byte[] sendData;

            if (sizeOfFragment < 1472) // otazka ci viac alebo 1024 ?  
            {
                sendData = new byte[sizeOfFragment]; //inicializuje na potrebnu velkost fragmentu
                int i = 0;
                
                for (i = 1; i < sizeOfFragment; i++)
                { 
                    sendData[i] = (byte)255; // nastavi od 1 do velkost na 255
                }
               



            
            }
        
        
        }



    }
}
