using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Dwms.Bll;
using Dwms.Web;

public partial class StopWords_Edit : System.Web.UI.Page
{
    string startedWord;
    string searchedWord;
    string systemId;          

    protected void Page_Load(object sender, EventArgs e)
    {
        startedWord = (string.IsNullOrEmpty(Request["startedWord"])) ? "All" : Request["startedWord"];
        searchedWord = (string.IsNullOrEmpty(Request["searchedWord"])) ? string.Empty : Request["searchedWord"];

        // Alphabet
        HyperLink allLink = new HyperLink();
        allLink.Text = "All";
        allLink.NavigateUrl = "./?startedWord=All" + "&searchedWord=" +
                searchedWord + "&systemId=" + systemId;

        if ("All" == startedWord)
            allLink.CssClass = "on";

        PlaceHolder1.Controls.Add(allLink);

        char[] alphaArr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        foreach (char c in alphaArr)
        {
            string alpha = c.ToString();
            HyperLink link = new HyperLink();
            link.Text = alpha;
            link.NavigateUrl = "./?startedWord=" + alpha + "&searchedWord=" +
                searchedWord + "&systemId=" + systemId;
            PlaceHolder1.Controls.Add(link);

            if (alpha == startedWord)
                link.CssClass = "on";
        }


        if (!IsPostBack)
        {
            StopWordDb Db = new StopWordDb();
            DataTable dt = Db.GetSearchWord(startedWord, searchedWord);

            foreach (DataRow r in dt.Rows)
            {
                if (StopWords.Text == string.Empty)
                    StopWords.Text += r["Word"].ToString();
                else
                    StopWords.Text += ", " + r["Word"].ToString();
            }

            HiddenOrigin.Value = StopWords.Text;
            SearchTextBox.Text = searchedWord;
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Edit.aspx?startedWord=" + startedWord + "&searchedWord=" + SearchTextBox.Text.Trim());
    }

    protected void Save(object sender, EventArgs e)
    {
        //delete stopword from hiddenfield, then add from text box
        char[] delimiterChars = { ',', ';', ' ' };
        string[] wordsNew = StopWords.Text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
        string[] wordsOrigin = HiddenOrigin.Value.Split(',');
        StopWordDb Db = new StopWordDb();
        
        foreach (string word in wordsOrigin)
        {
            Db.DeleteByWord(word.Trim());
        }

        foreach (string word in wordsNew)
        {
            if (!string.IsNullOrEmpty(word))
                Db.Insert(word.Trim());
        }
        Response.Redirect("Default.aspx?cfm=1&startedWord=" + startedWord + "&searchedWord=" + SearchTextBox.Text.Trim());
    }
}
