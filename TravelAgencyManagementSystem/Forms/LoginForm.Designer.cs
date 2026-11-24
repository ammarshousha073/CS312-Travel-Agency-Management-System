#nullable enable

using TravelAgencyManagementSystem.UserControls;

namespace TravelAgencyManagementSystem.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel mainLayout = null!;
    private TravelBrandPanel brandingPanel = null!;
    private Label brandTitleLabel = null!;
    private Label brandSubtitleLabel = null!;
    private Label brandFooterLabel = null!;
    private TableLayoutPanel signInLayout = null!;
    private Panel signInPanel = null!;
    private Label headingLabel = null!;
    private Label subtitleLabel = null!;
    private Label usernameLabel = null!;
    private Panel usernamePanel = null!;
    private TextBox usernameTextBox = null!;
    private Label passwordLabel = null!;
    private Panel passwordPanel = null!;
    private TextBox passwordTextBox = null!;
    private CheckBox showPasswordCheckBox = null!;
    private Button signInButton = null!;
    private LinkLabel forgotPasswordLink = null!;
    private Label previewLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        mainLayout = new TableLayoutPanel();
        brandingPanel = new TravelBrandPanel();
        brandTitleLabel = new Label();
        brandSubtitleLabel = new Label();
        brandFooterLabel = new Label();
        signInLayout = new TableLayoutPanel();
        signInPanel = new Panel();
        headingLabel = new Label();
        subtitleLabel = new Label();
        usernameLabel = new Label();
        usernamePanel = new Panel();
        usernameTextBox = new TextBox();
        passwordLabel = new Label();
        passwordPanel = new Panel();
        passwordTextBox = new TextBox();
        showPasswordCheckBox = new CheckBox();
        signInButton = new Button();
        forgotPasswordLink = new LinkLabel();
        previewLabel = new Label();
        mainLayout.SuspendLayout();
        brandingPanel.SuspendLayout();
        signInLayout.SuspendLayout();
        signInPanel.SuspendLayout();
        usernamePanel.SuspendLayout();
        passwordPanel.SuspendLayout();
        SuspendLayout();

        mainLayout.Size = new Size(1000, 640);
        mainLayout.ColumnCount = 2;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        mainLayout.RowCount = 1;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Margin = new Padding(0);
        mainLayout.Controls.Add(brandingPanel, 0, 0);
        mainLayout.Controls.Add(signInLayout, 1, 0);
        mainLayout.Name = "mainLayout";

        brandingPanel.Size = new Size(420, 640);
        brandingPanel.BackColor = Color.FromArgb(23, 43, 77);
        brandingPanel.Dock = DockStyle.Fill;
        brandingPanel.Margin = new Padding(0);
        brandingPanel.Name = "brandingPanel";
        brandingPanel.Controls.Add(brandTitleLabel);
        brandingPanel.Controls.Add(brandSubtitleLabel);
        brandingPanel.Controls.Add(brandFooterLabel);

        brandTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        brandTitleLabel.BackColor = Color.Transparent;
        brandTitleLabel.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
        brandTitleLabel.ForeColor = Color.White;
        brandTitleLabel.Location = new Point(28, 216);
        brandTitleLabel.Size = new Size(364, 136);
        brandTitleLabel.Text = "Travel Agency\nManagement System";
        brandTitleLabel.TextAlign = ContentAlignment.TopCenter;
        brandTitleLabel.Name = "brandTitleLabel";

        brandSubtitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        brandSubtitleLabel.BackColor = Color.Transparent;
        brandSubtitleLabel.Font = new Font("Segoe UI", 10F);
        brandSubtitleLabel.ForeColor = Color.FromArgb(195, 210, 228);
        brandSubtitleLabel.Location = new Point(24, 365);
        brandSubtitleLabel.Size = new Size(372, 46);
        brandSubtitleLabel.Text = "Trips  ·  Bookings  ·  Clients  ·  Payments";
        brandSubtitleLabel.TextAlign = ContentAlignment.TopCenter;
        brandSubtitleLabel.Name = "brandSubtitleLabel";

        brandFooterLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        brandFooterLabel.BackColor = Color.Transparent;
        brandFooterLabel.Font = new Font("Segoe UI", 9F);
        brandFooterLabel.ForeColor = Color.FromArgb(167, 189, 213);
        brandFooterLabel.Location = new Point(24, 587);
        brandFooterLabel.Size = new Size(372, 24);
        brandFooterLabel.Text = "Your next journey starts here.";
        brandFooterLabel.TextAlign = ContentAlignment.MiddleCenter;
        brandFooterLabel.Name = "brandFooterLabel";

        signInLayout.Size = new Size(580, 640);
        signInLayout.ColumnCount = 1;
        signInLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        signInLayout.RowCount = 3;
        signInLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        signInLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 480F));
        signInLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        signInLayout.Dock = DockStyle.Fill;
        signInLayout.Margin = new Padding(0);
        signInLayout.Padding = new Padding(64, 0, 64, 0);
        signInLayout.Controls.Add(signInPanel, 0, 1);
        signInLayout.Name = "signInLayout";

        signInPanel.Size = new Size(452, 480);
        signInPanel.Dock = DockStyle.Fill;
        signInPanel.Margin = new Padding(0);
        signInPanel.Name = "signInPanel";
        signInPanel.Controls.Add(headingLabel);
        signInPanel.Controls.Add(subtitleLabel);
        signInPanel.Controls.Add(usernameLabel);
        signInPanel.Controls.Add(usernamePanel);
        signInPanel.Controls.Add(passwordLabel);
        signInPanel.Controls.Add(passwordPanel);
        signInPanel.Controls.Add(showPasswordCheckBox);
        signInPanel.Controls.Add(signInButton);
        signInPanel.Controls.Add(forgotPasswordLink);
        signInPanel.Controls.Add(previewLabel);

        headingLabel.AutoSize = true;
        headingLabel.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
        headingLabel.Location = new Point(0, 8);
        headingLabel.Text = "Welcome back";
        headingLabel.Name = "headingLabel";

        subtitleLabel.AutoSize = true;
        subtitleLabel.Location = new Point(3, 64);
        subtitleLabel.Text = "Sign in to your travel agency workspace.";
        subtitleLabel.Name = "subtitleLabel";

        usernameLabel.AutoSize = true;
        usernameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        usernameLabel.Location = new Point(0, 113);
        usernameLabel.Text = "&Username";
        usernameLabel.TabIndex = 0;
        usernameLabel.Name = "usernameLabel";

        usernamePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        usernamePanel.Location = new Point(0, 141);
        usernamePanel.Size = new Size(452, 48);
        usernamePanel.TabIndex = 1;
        usernamePanel.Paint += InputPanel_Paint;
        usernamePanel.Controls.Add(usernameTextBox);
        usernamePanel.Name = "usernamePanel";

        usernameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        usernameTextBox.BorderStyle = BorderStyle.None;
        usernameTextBox.Font = new Font("Segoe UI", 11F);
        usernameTextBox.Location = new Point(15, 13);
        usernameTextBox.Size = new Size(422, 25);
        usernameTextBox.PlaceholderText = "Enter your username";
        usernameTextBox.AccessibleName = "Username";
        usernameTextBox.TabIndex = 0;
        usernameTextBox.Enter += InputTextBox_FocusChanged;
        usernameTextBox.Leave += InputTextBox_FocusChanged;
        usernameTextBox.Name = "usernameTextBox";

        passwordLabel.AutoSize = true;
        passwordLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        passwordLabel.Location = new Point(0, 211);
        passwordLabel.Text = "&Password";
        passwordLabel.TabIndex = 2;
        passwordLabel.Name = "passwordLabel";

        passwordPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        passwordPanel.Location = new Point(0, 239);
        passwordPanel.Size = new Size(452, 48);
        passwordPanel.TabIndex = 3;
        passwordPanel.Paint += InputPanel_Paint;
        passwordPanel.Controls.Add(passwordTextBox);
        passwordPanel.Name = "passwordPanel";

        passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        passwordTextBox.BorderStyle = BorderStyle.None;
        passwordTextBox.Font = new Font("Segoe UI", 11F);
        passwordTextBox.Location = new Point(15, 13);
        passwordTextBox.Size = new Size(422, 25);
        passwordTextBox.PlaceholderText = "Enter your password";
        passwordTextBox.UseSystemPasswordChar = true;
        passwordTextBox.AccessibleName = "Password";
        passwordTextBox.TabIndex = 0;
        passwordTextBox.Enter += InputTextBox_FocusChanged;
        passwordTextBox.Leave += InputTextBox_FocusChanged;
        passwordTextBox.Name = "passwordTextBox";

        showPasswordCheckBox.AutoSize = true;
        showPasswordCheckBox.Cursor = Cursors.Hand;
        showPasswordCheckBox.Location = new Point(1, 304);
        showPasswordCheckBox.Text = "&Show password";
        showPasswordCheckBox.TabIndex = 4;
        showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;
        showPasswordCheckBox.Name = "showPasswordCheckBox";

        signInButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        signInButton.FlatStyle = FlatStyle.Flat;
        signInButton.FlatAppearance.BorderSize = 0;
        signInButton.ForeColor = Color.White;
        signInButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        signInButton.Cursor = Cursors.Hand;
        signInButton.Location = new Point(0, 354);
        signInButton.Size = new Size(452, 48);
        signInButton.Text = "Sign in   →";
        signInButton.TabIndex = 5;
        signInButton.UseVisualStyleBackColor = false;
        signInButton.Click += SignInButton_Click;
        signInButton.Name = "signInButton";

        forgotPasswordLink.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        forgotPasswordLink.Location = new Point(0, 418);
        forgotPasswordLink.Size = new Size(452, 26);
        forgotPasswordLink.Text = "Forgot password?";
        forgotPasswordLink.TextAlign = ContentAlignment.MiddleCenter;
        forgotPasswordLink.TabIndex = 6;
        forgotPasswordLink.LinkClicked += ForgotPasswordLink_LinkClicked;
        forgotPasswordLink.Name = "forgotPasswordLink";

        previewLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        previewLabel.Font = new Font("Segoe UI", 9F);
        previewLabel.Location = new Point(0, 456);
        previewLabel.Size = new Size(452, 24);
        previewLabel.Text = "GUI preview  ·  No account required";
        previewLabel.TextAlign = ContentAlignment.MiddleCenter;
        previewLabel.Name = "previewLabel";

        AcceptButton = signInButton;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1000, 640);
        Controls.Add(mainLayout);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Travel Agency Management System — Sign in";
        Name = "LoginForm";
        mainLayout.ResumeLayout(false);
        brandingPanel.ResumeLayout(false);
        signInLayout.ResumeLayout(false);
        signInPanel.ResumeLayout(false);
        signInPanel.PerformLayout();
        usernamePanel.ResumeLayout(false);
        usernamePanel.PerformLayout();
        passwordPanel.ResumeLayout(false);
        passwordPanel.PerformLayout();
        ResumeLayout(false);
    }
}
