using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Controls_SplitDocument : System.Web.UI.UserControl
{
    int? id;
    //string radGridClientId;  Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //string cookieName;   Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //bool firstLoad;       Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

    public int DocumentId
    {
        get { return (id.HasValue ? id.Value : -1); }
        set { id = value; }
    }

    protected override void OnPreRender(EventArgs e)
    //protected void Page_Load(object sender, EventArgs e)
    {
        base.OnPreRender(e);

        PagesCheckBoxList.Items.Clear();
        SplitTypeRadioButtonList.Items.Clear();
        PopulateSplitType();

        if (DocumentId > 0)
        {
            RawPageDb rawPageDb = new RawPageDb();
            RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(DocumentId);

            if (rawPages.Count <= 1)
            {
                MessageLabel.Visible = true;
                SplitDocumentButton.Visible = SplitTypeLabel.Visible = SplitTypeRadioButtonList.Visible =  false;
                
            }
            else
            {
                MessageLabel.Visible = false;
                foreach (RawPage.RawPageRow rawPageRow in rawPages)
                {
                    PagesCheckBoxList.Items.Add(new ListItem("Page " + rawPageRow.DocPageNo.ToString(), rawPageRow.Id.ToString()));
                }
            }   
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void PopulateSplitType()
    {
        SplitTypeRadioButtonList.DataSource = Enum.GetValues(typeof(SplitTypeEnum));
        SplitTypeRadioButtonList.DataBind();
        SplitTypeRadioButtonList.SelectedIndex = 0;
    }

    protected void SplitDocumentButton_Click(object sender, EventArgs e)
    {
        string url = Request.Url.ToString().ToLower().Trim();
        if (DocumentId > 0)
        {
            List<int> pageIds = GetRawPageIds();

            if (pageIds.Count >= 1)
            {
                DocDb docDb = new DocDb();
                SplitTypeEnum splitType = (SplitTypeEnum)Enum.Parse(typeof(SplitTypeEnum), SplitTypeRadioButtonList.SelectedValue.Trim(), true);

                if (url.Contains("/verification/"))
                    docDb.SplitDocumentForVerification(DocumentId, pageIds, splitType);
                else
                    docDb.SplitDocumentForCompleteness(DocumentId, pageIds, splitType);
            }

            // Close the active tooltip
            ScriptManager.RegisterClientScriptBlock(
                this.Page,
                this.GetType(),
                "SaveDocInfoSript",
                "CloseActiveToolTip();",
                true);
        }
    }

    private List<int> GetRawPageIds()
    {
        List<int> rawPageIds = new List<int>();

        for (int i = 0; i < PagesCheckBoxList.Items.Count; i++)
        {
            if (PagesCheckBoxList.Items[i].Selected)
            {
                 rawPageIds.Add(int.Parse(PagesCheckBoxList.Items[i].Value));
            }
        }

        return rawPageIds;
    }
}