using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace Dwms.Web
{
    public class PassGen
    {
        protected Random rGen;
        protected string[] strCharacters = {"1","2","3","4","5","6","7","8","9","0"};

        public PassGen()
        {
            rGen = new Random();
        }

        public string GenPassLowercase(int i)
        {
            return GenPassWithCap(i).ToLower();
        }

        public string GenPassWithCap(int i)
        {
            string strPass = string.Empty;

            int p = 0;
            strPass = string.Empty;

            for (int x = 0; x < i; x++)
            {
                p = rGen.Next(0, 9);
                strPass += strCharacters[p];
            }
            
            return strPass;
        }
    }
}