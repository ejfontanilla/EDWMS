using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Web;
using Dwms.Bll;

public partial class Maintenance_OcrParameters_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");

        if (!IsPostBack)
        {
            ParameterDb parameterDb = new ParameterDb();

            //populate dropdown list
            PopulateMinimumAgeToProcessExternalFiles();
            PopulateMinimumEnglishWord();
            PopulateMinimumEnglishWordPercentage();
            PopulateMinimumScore();
            PopulateMinimumWordLength();
            PopulateKeywordCheckScope();
            PopulateMaxPagesDropDownList();
            PopulateMaxSampleDocsDropDownList();
            PopulateBinarize();
            PopulateMRCQuality();

            MaxSampleDocsDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MaxSampleDocs);
            ThreadDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MaximumThread);
            MaxPagesDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MaximumOcrPages);
            ExtFileAgeDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumAgeExternalFiles);
            TempFileAgeDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumAgeTempFiles);
            MinimumEnglishWordCountDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumEnglishWordCount);
            MinimumEnglishWordPercentageDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumEnglishWordPercentage);
            MinimumScoreDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumScore);
            MinimumWordLengthDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.MinimumWordLength);
            KeywordCheckScopeDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.KeywordCheckScope);
            SampleDocPercentDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.TopRankedSamplePages);

            BinarizeDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrBinarize);
            MorphDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrMorph);
            BackgroundFactorDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrBackgroundFactor);
            ForegroundFactorDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrForegroundFactor);
            QualityDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrQuality);
            DotMatrixList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrDotMatrix);
            DespeckleDropDownList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.OcrDespeckle);
        }

        // Set the access control for the user
        SetAccessControl();
    }

    private void PopulateMaxSampleDocsDropDownList()
    {
        for (double i = 100; i < 501; i += 50)
        {
            MaxSampleDocsDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMaxPagesDropDownList()
    {
        #region Modified by Edward 2014/06/12 to accomodate 1000 pages
        //for (double i = 60; i < 301; i += 20)
        for (double i = 100; i < 1501; i += 100)
        {
            MaxPagesDropDownList.Items.Add(i.ToString());
        }
        #endregion
    }

    private void PopulateKeywordCheckScope()
    {
        for (int i = 1; i < 6; i++)
        {
            KeywordCheckScopeDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMinimumWordLength()
    {
        for (int i = 1; i < 11; i++)
        {
            MinimumWordLengthDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMinimumScore()
    {
        for (decimal i = 0.01m; i <= 0.5m; i = i + 0.01m)
        {
            MinimumScoreDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMinimumEnglishWordPercentage()
    {
        for (int i = 1; i < 16; i++)
        {
            MinimumEnglishWordPercentageDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMinimumEnglishWord()
    {
        for (int i = 1; i < 11; i++)
        {
            MinimumEnglishWordCountDropDownList.Items.Add(i.ToString());
        }
    }

    private void PopulateMinimumAgeToProcessExternalFiles()
    {
        for (int i = 1; i < 21; i++)
			{
                ExtFileAgeDropDownList.Items.Add(i.ToString());
			}
    }

    private void PopulateBinarize()
    {
        for (int i = -1; i <= 200; i++)
        {
            BinarizeDropDownList.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }

    private void PopulateMRCQuality()
    {
        for (int i = 1; i <= 100; i++)
        {
            QualityDropDownList.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }

    /// <summary>
    /// Save parameter event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            ParameterDb parameterDb = new ParameterDb();

            parameterDb.Update(ParameterNameEnum.MaxSampleDocs, MaxSampleDocsDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MaximumThread, ThreadDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MaximumOcrPages, MaxPagesDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumAgeExternalFiles, ExtFileAgeDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumAgeTempFiles, TempFileAgeDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumEnglishWordCount, MinimumEnglishWordCountDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumEnglishWordPercentage, MinimumEnglishWordPercentageDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumScore, MinimumScoreDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.MinimumWordLength, MinimumWordLengthDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.KeywordCheckScope, KeywordCheckScopeDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.TopRankedSamplePages, SampleDocPercentDropDownList.SelectedValue);

            parameterDb.Update(ParameterNameEnum.OcrBinarize, BinarizeDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrMorph, MorphDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrBackgroundFactor, BackgroundFactorDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrForegroundFactor, ForegroundFactorDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrQuality, QualityDropDownList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrDotMatrix, DotMatrixList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.OcrDespeckle, DespeckleDropDownList.SelectedValue);

            Response.Redirect(Retrieve.GetPageName() + "?cfm=1");
        }
    }
    #endregion

    #region Validation
    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }
    #endregion
}
