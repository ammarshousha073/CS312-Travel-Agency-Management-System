using TravelAgencyManagementSystem.Data;
using TravelAgencyManagementSystem.Styling;

namespace TravelAgencyManagementSystem.Forms;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
        ApplyTheme();
        previewLabel.Text = "Local MongoDB sign-in";
    }

    private void ApplyTheme()
    {
        brandingPanel.BackColor = AppTheme.Navy;
        BackColor = Color.White;
        ForeColor = AppTheme.Text;
        headingLabel.ForeColor = AppTheme.Navy;
        subtitleLabel.ForeColor = AppTheme.SecondaryText;
        previewLabel.ForeColor = AppTheme.SecondaryText;
        signInButton.BackColor = AppTheme.Teal;
        signInButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(26, 146, 134);
        signInButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(22, 125, 115);
        forgotPasswordLink.LinkColor = AppTheme.Teal;
        forgotPasswordLink.ActiveLinkColor = AppTheme.Navy;
        usernamePanel.BackColor = AppTheme.Background;
        passwordPanel.BackColor = AppTheme.Background;
        usernameTextBox.BackColor = AppTheme.Background;
        passwordTextBox.BackColor = AppTheme.Background;
    }

    private void ShowPasswordCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        passwordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }

    private async void SignInButton_Click(object? sender, EventArgs e)
    {
        Account? account = null;
        if (!await UiOperation.RunAsync(this, async () =>
        {
            var store = AgencyStore.Current;
            await store.InitializeAsync();
            if (!await store.HasUsersAsync())
            {
                var username = RecordDialog.TextInput(usernameTextBox.Text);
                var email = RecordDialog.TextInput();
                var password = RecordDialog.TextInput();
                password.UseSystemPasswordChar = true;
                using var setup = new RecordDialog("Create administrator", "Choose credentials for the first account", false,
                    ("Username", username), ("Email", email), ("Password (12–256 characters)", password));
                setup.SaveAsync = () => store.CreateFirstAdministratorAsync(username.Text, email.Text, password.Text);
                if (setup.ShowDialog() == DialogResult.OK)
                {
                    usernameTextBox.Text = username.Text;
                    passwordTextBox.Clear();
                }
                return;
            }
            account = await store.SignInAsync(usernameTextBox.Text, passwordTextBox.Text);
        }) || account is null)
            return;

        using var mainForm = new MainForm { Account = account };
        Hide();
        try
        {
            mainForm.ShowDialog();
        }
        finally
        {
            passwordTextBox.Clear();
            showPasswordCheckBox.Checked = false;
            Show();
            usernameTextBox.Focus();
        }
    }

    private void ForgotPasswordLink_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        MessageBox.Show(this,
            "Contact your agency administrator for account assistance.",
            "Forgot password", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void InputPanel_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel)
            return;

        using var borderPen = new Pen(panel.ContainsFocus ? AppTheme.Teal : AppTheme.Border);
        e.Graphics.DrawRectangle(borderPen, 0, 0, panel.Width - 1, panel.Height - 1);
    }

    private void InputTextBox_FocusChanged(object? sender, EventArgs e)
    {
        if (sender is TextBox textBox)
            textBox.Parent?.Invalidate();
    }
}
