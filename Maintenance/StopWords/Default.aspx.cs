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
using Dwms.Dal;

public partial class StopWords_Default : System.Web.UI.Page
{
    string startedWord;
    string searchedWord;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
        }

        ConfirmPanel.Visible = (Request["cfm"] == "1");

        startedWord = (string.IsNullOrEmpty(Request["startedWord"])) ? "All" : Request["startedWord"];
        searchedWord = (string.IsNullOrEmpty(Request["searchedWord"])) ? string.Empty : Request["searchedWord"];

        EditButton.Attributes["onclick"] = "location.href='Edit.aspx?startedWord=" +
            startedWord + "&searchedWord=" + searchedWord + "'";

        if (!IsPostBack) SearchTextBox.Text = searchedWord;
       
        StopWordDb Db = new StopWordDb();
        DataTable dt = Db.GetSearchWord(startedWord, searchedWord);
        FilteredTokens.DataSource = dt;
        FilteredTokens.DataBind();

        // Alphabet
        HyperLink allLink = new HyperLink();
        allLink.Text = "All";
        allLink.NavigateUrl = "./?startedWord=All" + "&searchedWord=" +
                searchedWord;

        if ("All" == startedWord)
            allLink.CssClass = "on";
        
        PlaceHolder1.Controls.Add(allLink); 

        char[] alphaArr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        foreach (char c in alphaArr)
        {
            string alpha = c.ToString();
            HyperLink link = new HyperLink();
            link.Text = alpha;
            link.NavigateUrl = "./?startedWord=" + alpha + "&searchedWord=" ;  //ABC don't to clear the search box
            PlaceHolder1.Controls.Add(link);

            if (alpha == startedWord)
                link.CssClass = "on";
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("?startedWord=" + startedWord + "&searchedWord=" +
            SearchTextBox.Text.Trim());
    }
}
