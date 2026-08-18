using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AsotContentEditor;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new EditorForm());
    }
}

public sealed class EditorForm : Form
{
    private static readonly string DefaultProjectRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Documents", "Codex", "2026-06-03", "\u0421\u0430\u0439\u0442 \u0410\u0421\u041e\u0422");
    private static readonly string ProjectRootConfig = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ASOT", "content-editor-project.txt");
    private static string ProjectRoot = DefaultProjectRoot;

    private readonly Label _status = new();

    private readonly TextBox _certificateCaption = new();
    private readonly ComboBox _certificateColor = new();
    private readonly TextBox _certificateImage = new();
    private readonly ComboBox _certificateSelector = new();
    private Button? _addCertificateButton;

    private readonly TextBox _awardCaption = new();
    private readonly TextBox _awardImage = new();
    private readonly ComboBox _awardSelector = new();
    private Button? _addAwardButton;

    private readonly TextBox _reviewAuthor = new();
    private readonly TextBox _reviewText = new();
    private readonly ComboBox _reviewSelector = new();
    private Button? _addReviewButton;

    private readonly TextBox _clientTitle = new();
    private readonly TextBox _clientDescription = new();
    private readonly TextBox _clientServices = new();
    private readonly TextBox _clientLogo = new();
    private readonly TextBox _clientTextLogo = new();
    private readonly ComboBox _clientSelector = new();
    private Button? _addClientButton;

    private readonly TextBox _heroTitle = new();
    private readonly TextBox _heroText = new();
    private readonly TextBox _heroBackground = new();
    private readonly TextBox _aboutTitle = new();
    private readonly TextBox _aboutLead = new();
    private readonly TextBox _aboutText = new();
    private readonly TextBox _aboutImage = new();
    private readonly TextBox _missionNoteTitle = new();
    private readonly TextBox _missionNoteText = new();
    private readonly TextBox _missionTitle = new();
    private readonly TextBox _missionText = new();

    private readonly TextBox _companyPhone = new();
    private readonly TextBox _companyPhoneHref = new();
    private readonly TextBox _companyEmail = new();
    private readonly TextBox _companyEmailHref = new();
    private readonly TextBox _companyAddress = new();
    private readonly TextBox _companyAddressText = new();
    private readonly TextBox _companyDirector = new();
    private readonly TextBox _companyLatitude = new();
    private readonly TextBox _companyLongitude = new();

    private readonly TextBox _trustTitle = new();
    private readonly List<TextBox> _trustTexts = [];

    public EditorForm()
    {
        Text = "Редактор сайта АСОТ";
        Width = 940;
        Height = 780;
        MinimumSize = new Size(840, 660);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);
        SetCompanyIcon();
        ProjectRoot = ResolveProjectRoot();
        if (string.IsNullOrWhiteSpace(ProjectRoot))
        {
            BeginInvoke(Close);
            return;
        }

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(16, 8) };
        tabs.TabPages.Add(BuildTextsTab());
        tabs.TabPages.Add(BuildCompanyTab());
        tabs.TabPages.Add(BuildTrustTab());
        tabs.TabPages.Add(BuildClientsTab());
        tabs.TabPages.Add(BuildCertificatesTab());
        tabs.TabPages.Add(BuildAwardsTab());
        tabs.TabPages.Add(BuildReviewsTab());

        _status.Dock = DockStyle.Bottom;
        _status.Height = 44;
        _status.TextAlign = ContentAlignment.MiddleLeft;
        _status.Padding = new Padding(16, 0, 16, 0);
        _status.Text = "Готово к работе. После изменений обнови вкладку сайта на localhost:3000.";

        Controls.Add(tabs);
        Controls.Add(_status);

        LoadTextFields();
        LoadCompanyFields();
        LoadTrustFields();
        ReloadSelectors();
    }

    private TabPage BuildTextsTab()
    {
        var page = CreatePage("Тексты сайта");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Первый экран");
        AddTextField(panel, "Заголовок", _heroTitle);
        AddMultiline(panel, "Текст", _heroText, 80);
        AddImagePicker(panel, "Картинка шапки", _heroBackground, allowEmpty: true);

        AddTitle(panel, "О компании");
        AddTextField(panel, "Заголовок", _aboutTitle);
        AddMultiline(panel, "Жирный текст", _aboutLead, 90);
        AddMultiline(panel, "Основной текст", _aboutText, 110);
        AddImagePicker(panel, "Фото блока", _aboutImage, allowEmpty: true);

        AddTitle(panel, "Миссия");
        AddTextField(panel, "Заголовок плашки", _missionNoteTitle);
        AddMultiline(panel, "Текст плашки", _missionNoteText, 95);
        AddTextField(panel, "Заголовок миссии", _missionTitle);
        AddMultiline(panel, "Текст миссии", _missionText, 110);
        AddButton(panel, "Сохранить тексты", SaveTextBlocks, Color.FromArgb(35, 61, 134));

        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildCompanyTab()
    {
        var page = CreatePage("Контакты");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Контакты и карта");
        AddTextField(panel, "Телефон", _companyPhone);
        AddTextField(panel, "Ссылка телефона", _companyPhoneHref);
        AddTextField(panel, "Email", _companyEmail);
        AddTextField(panel, "Ссылка email", _companyEmailHref);
        AddTextField(panel, "Адрес для карты", _companyAddress);
        AddTextField(panel, "Адрес на сайте", _companyAddressText);
        AddTextField(panel, "Директор", _companyDirector);
        AddTextField(panel, "Широта карты", _companyLatitude);
        AddTextField(panel, "Долгота карты", _companyLongitude);
        AddButton(panel, "Сохранить контакты", SaveCompany, Color.FromArgb(35, 61, 134));

        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildTrustTab()
    {
        var page = CreatePage("Доверие");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Блок доверия");
        AddTextField(panel, "Заголовок блока", _trustTitle);

        for (var index = 0; index < 5; index++)
        {
            var box = new TextBox();
            _trustTexts.Add(box);
            AddMultiline(panel, $"Пункт {index + 1}", box, 72);
        }

        AddButton(panel, "Сохранить блок доверия", SaveTrust, Color.FromArgb(35, 61, 134));
        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildClientsTab()
    {
        var page = CreatePage("Наши клиенты");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Добавить или отредактировать клиента");
        AddTextField(panel, "Название", _clientTitle);
        AddMultiline(panel, "Описание", _clientDescription, 95);
        AddMultiline(panel, "Услуги (каждая с новой строки или через запятую)", _clientServices, 90);
        AddImagePicker(panel, "Новый логотип клиента", _clientLogo, allowEmpty: true);
        AddTextField(panel, "Текст вместо логотипа, если картинки нет", _clientTextLogo);
        _addClientButton = AddButton(panel, "Загрузить нового клиента", SaveClient, Color.FromArgb(35, 61, 134));

        AddTitle(panel, "Существующие клиенты");
        AddSelector(panel, "Выбери клиента", _clientSelector);
        _clientSelector.SelectedIndexChanged += (_, _) => LoadSelectedClient();
        AddButton(panel, "Новая запись / очистить выбор", ClearClientSelection, Color.DimGray);
        AddButton(panel, "Сохранить изменения клиента", SaveClientChanges, Color.FromArgb(35, 61, 134));
        AddButton(panel, "Удалить выбранного клиента", () => DeleteSelected("clients.js", "clients", _clientSelector, "клиента"), Color.Firebrick);

        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildCertificatesTab()
    {
        var page = CreatePage("Сертификаты");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Добавить сертификат");
        AddTextField(panel, "Подпись сертификата", _certificateCaption);
        _certificateColor.DropDownStyle = ComboBoxStyle.DropDownList;
        _certificateColor.Items.AddRange(["blue - синяя", "navy - темно-синяя", "gray - серая", "pink - розовая"]);
        _certificateColor.SelectedIndex = 0;
        AddControl(panel, "Цвет рамки", _certificateColor);
        AddImagePicker(panel, "Картинка сертификата", _certificateImage);
        _addCertificateButton = AddButton(panel, "Загрузить сертификат", SaveCertificate, Color.FromArgb(35, 61, 134));

        AddTitle(panel, "Существующие сертификаты");
        AddSelector(panel, "Список сертификатов", _certificateSelector);
        _certificateSelector.SelectedIndexChanged += (_, _) => LoadSelectedCertificate();
        AddButton(panel, "Новая запись / очистить выбор", ClearCertificateSelection, Color.DimGray);
        AddButton(panel, "Сохранить изменения сертификата", SaveCertificateChanges, Color.FromArgb(35, 61, 134));
        AddButton(panel, "Удалить выбранный сертификат", () => DeleteSelected("certificates.js", "certificates", _certificateSelector, "сертификат"), Color.Firebrick);

        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildAwardsTab()
    {
        var page = CreatePage("Грамоты");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Добавить грамоту");
        AddTextField(panel, "Подпись грамоты", _awardCaption);
        AddImagePicker(panel, "Картинка грамоты", _awardImage);
        _addAwardButton = AddButton(panel, "Загрузить грамоту", SaveAward, Color.FromArgb(35, 61, 134));

        AddTitle(panel, "Существующие грамоты");
        AddSelector(panel, "Список грамот", _awardSelector);
        _awardSelector.SelectedIndexChanged += (_, _) => LoadSelectedAward();
        AddButton(panel, "Новая запись / очистить выбор", ClearAwardSelection, Color.DimGray);
        AddButton(panel, "Сохранить изменения грамоты", SaveAwardChanges, Color.FromArgb(35, 61, 134));
        AddButton(panel, "Удалить выбранную грамоту", () => DeleteSelected("awards.js", "awards", _awardSelector, "грамоту"), Color.Firebrick);

        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildReviewsTab()
    {
        var page = CreatePage("Отзывы");
        var panel = CreatePanel();

        AddLogo(panel);
        AddTitle(panel, "Добавить или отредактировать отзыв");
        AddTextField(panel, "Автор / компания", _reviewAuthor);
        AddMultiline(panel, "Текст отзыва", _reviewText, 150);
        _addReviewButton = AddButton(panel, "Загрузить новый отзыв", SaveReview, Color.FromArgb(35, 61, 134));

        AddTitle(panel, "Существующие отзывы");
        AddSelector(panel, "Отзывы по авторам", _reviewSelector);
        _reviewSelector.SelectedIndexChanged += (_, _) => LoadSelectedReview();
        AddButton(panel, "Новая запись / очистить выбор", ClearReviewSelection, Color.DimGray);
        AddButton(panel, "Сохранить изменения отзыва", SaveReviewChanges, Color.FromArgb(35, 61, 134));
        AddButton(panel, "Удалить выбранный отзыв", () => DeleteSelected("reviews.js", "reviews", _reviewSelector, "отзыв"), Color.Firebrick);

        page.Controls.Add(panel);
        return page;
    }

    private static TabPage CreatePage(string title) => new(title) { BackColor = Color.White, Padding = new Padding(20) };

    private void SetCompanyIcon()
    {
        var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        if (icon is not null) Icon = icon;
    }

    private static string ResolveProjectRoot()
    {
        foreach (var candidate in ProjectRootCandidates())
        {
            if (IsProjectRoot(candidate)) return candidate;
        }

        MessageBox.Show(
            "Не нашёл папку проекта сайта. Выбери папку, где лежат editable-data, src и package.json.\n\nДля работы по локальной сети можно выбрать сетевой путь вида \\\\КОМПЬЮТЕР\\Папка\\Сайт АСОТ.",
            "Редактор сайта АСОТ",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        using var dialog = new FolderBrowserDialog
        {
            Description = "Выбери папку проекта сайта АСОТ",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false
        };

        if (dialog.ShowDialog() != DialogResult.OK) return "";
        if (!IsProjectRoot(dialog.SelectedPath))
        {
            MessageBox.Show(
                "В выбранной папке не найдены editable-data, src и package.json. Редактор закроется.",
                "Папка проекта не подходит",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return "";
        }

        SaveProjectRoot(dialog.SelectedPath);
        return dialog.SelectedPath;
    }

    private static IEnumerable<string> ProjectRootCandidates()
    {
        var exeConfig = Path.Combine(AppContext.BaseDirectory, "asot-project-path.txt");
        if (File.Exists(exeConfig)) yield return File.ReadAllText(exeConfig, Encoding.UTF8).Trim();
        if (File.Exists(ProjectRootConfig)) yield return File.ReadAllText(ProjectRootConfig, Encoding.UTF8).Trim();
        yield return DefaultProjectRoot;
        yield return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }

    private static bool IsProjectRoot(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return Directory.Exists(path)
            && Directory.Exists(Path.Combine(path, "editable-data"))
            && Directory.Exists(Path.Combine(path, "src"))
            && File.Exists(Path.Combine(path, "package.json"));
    }

    private static void SaveProjectRoot(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ProjectRootConfig)!);
        File.WriteAllText(ProjectRootConfig, path, new UTF8Encoding(false));
    }

    private static Panel CreatePanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(8)
        };

        panel.SizeChanged += (_, _) => FitPanelChildren(panel);
        panel.ControlAdded += (_, _) =>
        {
            FitPanelChildren(panel);
            UpdateScrollSize(panel);
        };

        return panel;
    }

    private static int NextTop(Panel panel)
    {
        if (panel.Controls.Count == 0) return panel.Padding.Top;

        return panel.Controls
            .Cast<Control>()
            .Max(control => control.Bottom) + 8;
    }

    private static void FitPanelChildren(Panel panel)
    {
        var width = Math.Max(240, panel.ClientSize.Width - panel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 12);

        foreach (Control control in panel.Controls)
        {
            if (Equals(control.Tag, "fixed-logo"))
            {
                control.Left = Math.Max(panel.Padding.Left, panel.ClientSize.Width - control.Width - panel.Padding.Right - SystemInformation.VerticalScrollBarWidth - 12);
                continue;
            }

            control.Width = width;
        }
    }

    private static void UpdateScrollSize(Panel panel)
    {
        var bottom = panel.Controls.Count == 0 ? 0 : panel.Controls.Cast<Control>().Max(control => control.Bottom);
        panel.AutoScrollMinSize = new Size(0, bottom + panel.Padding.Bottom + 16);
    }

    private static void PrepareControl(Panel panel, Control control, int height)
    {
        control.Left = panel.Padding.Left;
        control.Top = NextTop(panel);
        control.Width = Math.Max(240, panel.ClientSize.Width - panel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 12);
        control.Height = height;
        control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        control.Margin = Padding.Empty;
    }

    private static void AddTitle(Panel panel, string text)
    {
        var title = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        PrepareControl(panel, title, 48);
        panel.Controls.Add(title);
    }

    private static void AddLogo(Panel panel)
    {
        var logo = new PictureBox
        {
            Image = LoadLogoImage(),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Tag = "fixed-logo"
        };

        logo.Width = 58;
        logo.Height = 72;
        logo.Top = NextTop(panel);
        logo.Left = Math.Max(panel.Padding.Left, panel.ClientSize.Width - logo.Width - panel.Padding.Right - SystemInformation.VerticalScrollBarWidth - 12);
        logo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        panel.Controls.Add(logo);
    }

    private static Image LoadLogoImage()
    {
        var resourceName = typeof(EditorForm).Assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(".logo.png", StringComparison.OrdinalIgnoreCase));

        if (resourceName is not null)
        {
            using var stream = typeof(EditorForm).Assembly.GetManifestResourceStream(resourceName);
            if (stream is not null)
            {
                using var source = Image.FromStream(stream);
                return new Bitmap(source);
            }
        }

        var path = Path.Combine(AppContext.BaseDirectory, "logo.png");
        if (File.Exists(path))
        {
            using var source = Image.FromFile(path);
            return new Bitmap(source);
        }

        var fallback = new Bitmap(40, 66);
        using var graphics = Graphics.FromImage(fallback);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);
        using var blue = new SolidBrush(Color.FromArgb(35, 61, 134));
        using var accent = new SolidBrush(Color.FromArgb(37, 134, 197));
        graphics.FillPolygon(blue, [new Point(18, 0), new Point(0, 58), new Point(18, 48)]);
        graphics.FillPolygon(accent, [new Point(20, 18), new Point(40, 30), new Point(20, 66)]);
        return fallback;
    }

    private static void AddTextField(Panel panel, string label, TextBox textBox)
    {
        AddControl(panel, label, textBox);
    }

    private static void AddMultiline(Panel panel, string label, TextBox textBox, int height)
    {
        textBox.Multiline = true;
        textBox.ScrollBars = ScrollBars.Vertical;
        AddControl(panel, label, textBox, height);
    }

    private static void AddSelector(Panel panel, string label, ComboBox selector)
    {
        selector.DropDownStyle = ComboBoxStyle.DropDownList;
        AddControl(panel, label, selector);
    }

    private static void AddControl(Panel panel, string label, Control control, int controlHeight = 34)
    {
        var labelControl = new Label { Text = label, TextAlign = ContentAlignment.BottomLeft };
        PrepareControl(panel, labelControl, 28);
        panel.Controls.Add(labelControl);

        PrepareControl(panel, control, controlHeight);
        control.Top += 2;
        panel.Controls.Add(control);
    }

    private static void AddImagePicker(Panel panel, string label, TextBox target, bool allowEmpty = false)
    {
        target.ReadOnly = true;
        var row = new TableLayoutPanel { ColumnCount = 3 };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, allowEmpty ? 90 : 0));

        var pick = new Button { Text = "Выбрать...", Dock = DockStyle.Fill };
        pick.Click += (_, _) => PickImage(target);
        target.Dock = DockStyle.Fill;
        row.Controls.Add(target, 0, 0);
        row.Controls.Add(pick, 1, 0);

        if (allowEmpty)
        {
            var clear = new Button { Text = "Очистить", Dock = DockStyle.Fill };
            clear.Click += (_, _) => target.Clear();
            row.Controls.Add(clear, 2, 0);
        }

        var labelControl = new Label { Text = label, TextAlign = ContentAlignment.BottomLeft };
        PrepareControl(panel, labelControl, 28);
        panel.Controls.Add(labelControl);

        PrepareControl(panel, row, 38);
        row.Top += 2;
        panel.Controls.Add(row);
    }

    private static Button AddButton(Panel panel, string text, Action action, Color color)
    {
        var button = new Button { Text = text, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => action();
        PrepareControl(panel, button, 44);
        panel.Controls.Add(button);
        return button;
    }

    private static void PickImage(TextBox target)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Выбери картинку",
            Filter = "Картинки|*.jpg;*.jpeg;*.png;*.webp;*.gif;*.svg|Все файлы|*.*"
        };

        if (dialog.ShowDialog() == DialogResult.OK) target.Text = dialog.FileName;
    }

    private void SaveCertificate()
    {
        RunSave(() =>
        {
            Require(_certificateCaption.Text, "Заполни подпись сертификата.");
            Require(_certificateImage.Text, "Выбери картинку сертификата.");
            var image = CopyImageToProject(_certificateImage.Text);
            var color = _certificateColor.Text.Split(' ')[0];
            AppendToDataArray("certificates.js", BuildCertificateItem(image, color, _certificateCaption.Text));
            ClearFields(_certificateCaption, _certificateImage);
            ReloadSelectors();
            SetCertificateCreateMode();
        });
    }

    private void SaveAward()
    {
        RunSave(() =>
        {
            Require(_awardCaption.Text, "Заполни подпись грамоты.");
            Require(_awardImage.Text, "Выбери картинку грамоты.");
            var image = CopyImageToProject(_awardImage.Text);
            AppendToDataArray("awards.js", BuildAwardItem(image, _awardCaption.Text));
            ClearFields(_awardCaption, _awardImage);
            ReloadSelectors();
            SetAwardCreateMode();
        });
    }

    private void SaveCertificateChanges()
    {
        if (_certificateSelector.SelectedItem is not Entry selected)
        {
            SetStatus("Выбери сертификат для редактирования.", true);
            return;
        }

        RunSave(() =>
        {
            Require(_certificateCaption.Text, "Заполни подпись сертификата.");
            Require(_certificateImage.Text, "Выбери картинку сертификата.");
            var image = ResolveImageValue(_certificateImage.Text);
            var color = _certificateColor.Text.Split(' ')[0];
            ReplaceArrayItem("certificates.js", "certificates", selected.Index, BuildCertificateItem(image, color, _certificateCaption.Text));
            ReloadSelectors();
            ClearCertificateSelection();
        }, "Готово. Сертификат изменён. Обнови вкладку сайта на localhost:3000.");
    }

    private void SaveAwardChanges()
    {
        if (_awardSelector.SelectedItem is not Entry selected)
        {
            SetStatus("Выбери грамоту для редактирования.", true);
            return;
        }

        RunSave(() =>
        {
            Require(_awardCaption.Text, "Заполни подпись грамоты.");
            Require(_awardImage.Text, "Выбери картинку грамоты.");
            var image = ResolveImageValue(_awardImage.Text);
            ReplaceArrayItem("awards.js", "awards", selected.Index, BuildAwardItem(image, _awardCaption.Text));
            ReloadSelectors();
            ClearAwardSelection();
        }, "Готово. Грамота изменена. Обнови вкладку сайта на localhost:3000.");
    }

    private void SaveReview()
    {
        RunSave(() =>
        {
            Require(_reviewAuthor.Text, "Заполни автора отзыва.");
            Require(_reviewText.Text, "Заполни текст отзыва.");
            AppendToDataArray("reviews.js", BuildReviewItem(_reviewAuthor.Text, _reviewText.Text));
            ClearFields(_reviewAuthor, _reviewText);
            ReloadSelectors();
            SetReviewCreateMode();
        });
    }

    private void SaveReviewChanges()
    {
        if (_reviewSelector.SelectedItem is not Entry selected)
        {
            SetStatus("Выбери отзыв для редактирования.", true);
            return;
        }

        RunSave(() =>
        {
            Require(_reviewAuthor.Text, "Заполни автора отзыва.");
            Require(_reviewText.Text, "Заполни текст отзыва.");
            ReplaceArrayItem("reviews.js", "reviews", selected.Index, BuildReviewItem(_reviewAuthor.Text, _reviewText.Text));
            ReloadSelectors();
            ClearReviewSelection();
        }, "Готово. Отзыв изменён. Обнови вкладку сайта на localhost:3000.");
    }

    private void SaveClient()
    {
        RunSave(() =>
        {
            AppendToDataArray("clients.js", BuildClientItem());
            ClearClientFields();
            ReloadSelectors();
            SetClientCreateMode();
        });
    }

    private void SaveClientChanges()
    {
        if (_clientSelector.SelectedItem is not Entry selected)
        {
            SetStatus("Выбери клиента для редактирования.", true);
            return;
        }

        RunSave(() =>
        {
            ReplaceArrayItem("clients.js", "clients", selected.Index, BuildClientItem(LoadExistingClientImage(selected.Index)));
            ReloadSelectors();
            ClearClientSelection();
        }, "Готово. Клиент изменён. Обнови вкладку сайта на localhost:3000.");
    }

    private string BuildClientItem(string? existingImage = null)
    {
        Require(_clientTitle.Text, "Заполни название клиента.");
        Require(_clientDescription.Text, "Заполни описание клиента.");

        var services = SplitLines(_clientServices.Text);
        if (services.Count == 0) throw new InvalidOperationException("Добавь хотя бы одну услугу клиента.");

        var imageLines = "";
        if (!string.IsNullOrWhiteSpace(_clientLogo.Text))
        {
            var image = File.Exists(_clientLogo.Text) ? CopyImageToProject(_clientLogo.Text) : _clientLogo.Text.Trim();
            imageLines = $"""
                logoMono: "{Escape(image)}",
                logoColor: "{Escape(image)}",
                alt: "{Escape(_clientTitle.Text.Trim())}",
            """;
        }
        else if (!string.IsNullOrWhiteSpace(existingImage))
        {
            imageLines = $"""
                logoMono: "{Escape(existingImage)}",
                logoColor: "{Escape(existingImage)}",
                alt: "{Escape(_clientTitle.Text.Trim())}",
            """;
        }
        else
        {
            Require(_clientTextLogo.Text, "Выбери логотип или заполни текст вместо логотипа.");
            imageLines = $"""    textLogo: "{Escape(_clientTextLogo.Text.Trim())}",""";
        }

        return $$"""
          {
            title: "{{Escape(_clientTitle.Text.Trim())}}",
        {{imageLines}}
            description: "{{Escape(_clientDescription.Text.Trim())}}",
            services: [{{string.Join(", ", services.Select(service => $"\"{Escape(service)}\""))}}]
          }
        """;
    }

    private void SaveTextBlocks()
    {
        RunSave(() =>
        {
            ReplaceStringProperty("hero.js", "title", _heroTitle.Text);
            ReplaceStringProperty("hero.js", "text", _heroText.Text);
            SaveOptionalImage("hero.js", "backgroundImage", _heroBackground.Text);
            ReplaceStringProperty("about.js", "title", _aboutTitle.Text);
            ReplaceStringProperty("about.js", "lead", _aboutLead.Text);
            ReplaceStringProperty("about.js", "text", _aboutText.Text);
            SaveOptionalImage("about.js", "image", _aboutImage.Text);
            ReplaceStringProperty("mission.js", "noteTitle", _missionNoteTitle.Text);
            ReplaceStringProperty("mission.js", "noteText", _missionNoteText.Text);
            ReplaceStringProperty("mission.js", "title", _missionTitle.Text);
            ReplaceStringProperty("mission.js", "text", _missionText.Text);
        }, "Готово. Тексты сайта сохранены. Обнови вкладку сайта на localhost:3000.");
    }

    private void SaveCompany()
    {
        RunSave(() =>
        {
            ReplaceStringProperty("company.js", "phone", _companyPhone.Text);
            ReplaceStringProperty("company.js", "phoneHref", _companyPhoneHref.Text);
            ReplaceStringProperty("company.js", "email", _companyEmail.Text);
            ReplaceStringProperty("company.js", "emailHref", _companyEmailHref.Text);
            ReplaceStringProperty("company.js", "address", _companyAddress.Text);
            ReplaceStringProperty("company.js", "addressText", _companyAddressText.Text);
            ReplaceStringProperty("company.js", "director", _companyDirector.Text);
            ReplaceCoordinates(_companyLatitude.Text, _companyLongitude.Text);
        }, "Готово. Контакты сохранены. Обнови вкладку сайта на localhost:3000.");
    }

    private void SaveTrust()
    {
        RunSave(() =>
        {
            ReplaceStringProperty("trust.js", "title", _trustTitle.Text);
            ReplaceTrustTexts(_trustTexts.Select(box => box.Text).ToList());
        }, "Готово. Блок доверия сохранён. Обнови вкладку сайта на localhost:3000.");
    }

    private void DeleteSelected(string fileName, string exportName, ComboBox selector, string label)
    {
        if (selector.SelectedItem is not Entry selected)
        {
            SetStatus($"Выбери {label} для удаления.", true);
            return;
        }

        if (MessageBox.Show($"Удалить: {selected.Title}?", $"Удаление: {label}", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        RunSave(() =>
        {
            RemoveArrayItem(fileName, exportName, selected.Index);
            ReloadSelectors();
        }, $"Готово. {UpperFirst(label)} удалён. Обнови вкладку сайта на localhost:3000.");
    }

    private void LoadSelectedReview()
    {
        if (_reviewSelector.SelectedItem is not Entry selected) return;
        var block = ReadArrayBlocks("reviews.js")[selected.Index];
        _reviewAuthor.Text = ReadJsStringProperty(block, "author");
        _reviewText.Text = ReadJsStringProperty(block, "text");
        SetReviewEditMode();
    }

    private void LoadSelectedCertificate()
    {
        if (_certificateSelector.SelectedItem is not Entry selected) return;
        var block = ReadArrayBlocks("certificates.js")[selected.Index];
        _certificateCaption.Text = ReadJsStringProperty(block, "caption");
        _certificateImage.Text = ReadJsStringProperty(block, "image");
        SelectColor(ReadJsStringProperty(block, "color"));
        SetCertificateEditMode();
    }

    private void LoadSelectedAward()
    {
        if (_awardSelector.SelectedItem is not Entry selected) return;
        var block = ReadArrayBlocks("awards.js")[selected.Index];
        _awardCaption.Text = ReadJsStringProperty(block, "caption");
        _awardImage.Text = ReadJsStringProperty(block, "image");
        SetAwardEditMode();
    }

    private void SelectColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color)) return;
        for (var index = 0; index < _certificateColor.Items.Count; index++)
        {
            if (_certificateColor.Items[index]?.ToString()?.StartsWith(color + " ", StringComparison.OrdinalIgnoreCase) == true)
            {
                _certificateColor.SelectedIndex = index;
                return;
            }
        }
    }

    private void LoadSelectedClient()
    {
        if (_clientSelector.SelectedItem is not Entry selected) return;
        var block = ReadArrayBlocks("clients.js")[selected.Index];
        _clientTitle.Text = ReadJsStringProperty(block, "title");
        _clientDescription.Text = ReadJsStringProperty(block, "description");
        _clientLogo.Text = ReadJsStringProperty(block, "logoMono");
        _clientTextLogo.Text = ReadJsStringProperty(block, "textLogo");
        _clientServices.Text = string.Join(Environment.NewLine, ReadServices(block));
        SetClientEditMode();
    }

    private string LoadExistingClientImage(int index)
    {
        var block = ReadArrayBlocks("clients.js")[index];
        return ReadJsStringProperty(block, "logoMono");
    }

    private void ClearClientFields()
    {
        ClearFields(_clientTitle, _clientDescription, _clientServices, _clientLogo, _clientTextLogo);
    }

    private void ClearClientSelection()
    {
        _clientSelector.SelectedIndex = -1;
        ClearClientFields();
        SetClientCreateMode();
        SetStatus("Режим новой записи клиента. Кнопка загрузки снова активна.", false);
    }

    private void ClearReviewSelection()
    {
        _reviewSelector.SelectedIndex = -1;
        ClearFields(_reviewAuthor, _reviewText);
        SetReviewCreateMode();
        SetStatus("Режим нового отзыва. Кнопка загрузки снова активна.", false);
    }

    private void ClearCertificateSelection()
    {
        _certificateSelector.SelectedIndex = -1;
        ClearFields(_certificateCaption, _certificateImage);
        if (_certificateColor.Items.Count > 0) _certificateColor.SelectedIndex = 0;
        SetCertificateCreateMode();
        SetStatus("Режим нового сертификата. Кнопка загрузки снова активна.", false);
    }

    private void ClearAwardSelection()
    {
        _awardSelector.SelectedIndex = -1;
        ClearFields(_awardCaption, _awardImage);
        SetAwardCreateMode();
        SetStatus("Режим новой грамоты. Кнопка загрузки снова активна.", false);
    }

    private void SetClientEditMode()
    {
        if (_addClientButton is not null) _addClientButton.Enabled = false;
    }

    private void SetClientCreateMode()
    {
        if (_addClientButton is not null) _addClientButton.Enabled = true;
    }

    private void SetReviewEditMode()
    {
        if (_addReviewButton is not null) _addReviewButton.Enabled = false;
    }

    private void SetReviewCreateMode()
    {
        if (_addReviewButton is not null) _addReviewButton.Enabled = true;
    }

    private void SetCertificateEditMode()
    {
        if (_certificateSelector.SelectedItem is null)
        {
            SetCertificateCreateMode();
            return;
        }

        if (_addCertificateButton is not null) _addCertificateButton.Enabled = false;
    }

    private void SetCertificateCreateMode()
    {
        if (_addCertificateButton is not null) _addCertificateButton.Enabled = true;
    }

    private void SetAwardEditMode()
    {
        if (_awardSelector.SelectedItem is null)
        {
            SetAwardCreateMode();
            return;
        }

        if (_addAwardButton is not null) _addAwardButton.Enabled = false;
    }

    private void SetAwardCreateMode()
    {
        if (_addAwardButton is not null) _addAwardButton.Enabled = true;
    }

    private void RunSave(Action save, string? successMessage = null)
    {
        try
        {
            save();
            TouchDevServerEntry();
            SetStatus(successMessage ?? "Готово. Данные добавлены. Обнови вкладку сайта на localhost:3000.", false);
        }
        catch (Exception error)
        {
            SetStatus(error.Message, true);
        }
    }

    private void SetStatus(string text, bool isError)
    {
        _status.ForeColor = isError ? Color.Firebrick : Color.FromArgb(23, 106, 52);
        _status.Text = text;
    }

    private void ReloadSelectors()
    {
        FillSelector(_certificateSelector, "certificates.js", "caption");
        FillSelector(_awardSelector, "awards.js", "caption");
        FillSelector(_reviewSelector, "reviews.js", "author");
        FillSelector(_clientSelector, "clients.js", "title");
        if (_certificateSelector.SelectedIndex < 0) SetCertificateCreateMode();
        if (_awardSelector.SelectedIndex < 0) SetAwardCreateMode();
        if (_reviewSelector.SelectedIndex < 0) SetReviewCreateMode();
        if (_clientSelector.SelectedIndex < 0) SetClientCreateMode();
    }

    private static void FillSelector(ComboBox selector, string fileName, string titleProperty)
    {
        selector.Items.Clear();
        var blocks = ReadArrayBlocks(fileName);

        for (var index = 0; index < blocks.Count; index++)
        {
            var title = ReadJsStringProperty(blocks[index], titleProperty);
            selector.Items.Add(new Entry(index, string.IsNullOrWhiteSpace(title) ? $"Запись {index + 1}" : title));
        }
    }

    private void LoadTextFields()
    {
        _heroTitle.Text = ReadStringProperty("hero.js", "title");
        _heroText.Text = ReadStringProperty("hero.js", "text");
        _heroBackground.Text = ReadStringProperty("hero.js", "backgroundImage");
        _aboutTitle.Text = ReadStringProperty("about.js", "title");
        _aboutLead.Text = ReadStringProperty("about.js", "lead");
        _aboutText.Text = ReadStringProperty("about.js", "text");
        _aboutImage.Text = ReadStringProperty("about.js", "image");
        _missionNoteTitle.Text = ReadStringProperty("mission.js", "noteTitle");
        _missionNoteText.Text = ReadStringProperty("mission.js", "noteText");
        _missionTitle.Text = ReadStringProperty("mission.js", "title");
        _missionText.Text = ReadStringProperty("mission.js", "text");
    }

    private void LoadCompanyFields()
    {
        _companyPhone.Text = ReadStringProperty("company.js", "phone");
        _companyPhoneHref.Text = ReadStringProperty("company.js", "phoneHref");
        _companyEmail.Text = ReadStringProperty("company.js", "email");
        _companyEmailHref.Text = ReadStringProperty("company.js", "emailHref");
        _companyAddress.Text = ReadStringProperty("company.js", "address");
        _companyAddressText.Text = ReadStringProperty("company.js", "addressText");
        _companyDirector.Text = ReadStringProperty("company.js", "director");
        var coordinates = ReadCoordinates();
        _companyLatitude.Text = coordinates.latitude;
        _companyLongitude.Text = coordinates.longitude;
    }

    private void LoadTrustFields()
    {
        _trustTitle.Text = ReadStringProperty("trust.js", "title");
        var blocks = ReadArrayBlocksFromFile("trust.js");
        for (var index = 0; index < _trustTexts.Count && index < blocks.Count; index++)
        {
            _trustTexts[index].Text = ReadJsStringProperty(blocks[index], "text");
        }
    }

    private static void Require(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException(message);
    }

    private static void ClearFields(params TextBox[] fields)
    {
        foreach (var field in fields) field.Clear();
    }

    private static string BuildCertificateItem(string image, string color, string caption) => $$"""
      {
        image: "{{Escape(image)}}",
        color: "{{Escape(color)}}",
        caption: "{{Escape(caption.Trim())}}"
      }
    """;

    private static string BuildAwardItem(string image, string caption) => $$"""
      {
        image: "{{Escape(image)}}",
        caption: "{{Escape(caption.Trim())}}"
      }
    """;

    private static string BuildReviewItem(string author, string text) => $$"""
      {
        author: "{{Escape(author.Trim())}}",
        text: "{{Escape(text.Trim())}}"
      }
    """;

    private static string CopyImageToProject(string sourcePath)
    {
        var imagesDir = Path.Combine(ProjectRoot, "src", "images");
        Directory.CreateDirectory(imagesDir);
        var extension = Path.GetExtension(sourcePath);
        if (string.IsNullOrWhiteSpace(extension)) extension = ".jpg";
        var name = Path.GetFileNameWithoutExtension(sourcePath).ToLowerInvariant();
        name = Regex.Replace(name, @"[^\p{L}\p{Nd}]+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(name)) name = "image";
        var filename = $"{name}-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}{extension.ToLowerInvariant()}";
        var target = Path.Combine(imagesDir, filename);
        File.Copy(sourcePath, target, overwrite: false);
        return $"images/{filename}";
    }

    private static string ResolveImageValue(string value)
    {
        var trimmed = value.Trim();
        return File.Exists(trimmed) ? CopyImageToProject(trimmed) : trimmed;
    }

    private static void SaveOptionalImage(string fileName, string property, string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && File.Exists(value))
        {
            ReplaceStringProperty(fileName, property, CopyImageToProject(value));
            return;
        }
        if (!string.IsNullOrWhiteSpace(value)) ReplaceStringProperty(fileName, property, value);
    }

    private static void AppendToDataArray(string fileName, string item)
    {
        var file = DataPath(fileName);
        var text = File.ReadAllText(file, Encoding.UTF8);
        var index = text.LastIndexOf("];", StringComparison.Ordinal);
        if (index < 0) throw new InvalidOperationException($"Не нашёл массив данных в файле {fileName}.");
        var before = text[..index].TrimEnd();
        var after = text[index..];
        var separator = before.EndsWith("[", StringComparison.Ordinal) ? "\n" : ",\n";
        File.WriteAllText(file, $"{before}{separator}{item}\n{after}", new UTF8Encoding(false));
    }

    private static void ReplaceArrayItem(string fileName, string exportName, int replaceIndex, string item)
    {
        var blocks = ReadArrayBlocks(fileName);
        if (replaceIndex < 0 || replaceIndex >= blocks.Count) throw new InvalidOperationException("Запись уже не найдена. Обнови список.");
        blocks[replaceIndex] = item.Trim();
        WriteArray(fileName, exportName, blocks);
    }

    private static void RemoveArrayItem(string fileName, string exportName, int removeIndex)
    {
        var blocks = ReadArrayBlocks(fileName);
        if (removeIndex < 0 || removeIndex >= blocks.Count) throw new InvalidOperationException("Запись уже не найдена. Обнови список.");
        blocks.RemoveAt(removeIndex);
        WriteArray(fileName, exportName, blocks);
    }

    private static void WriteArray(string fileName, string exportName, List<string> blocks)
    {
        var file = DataPath(fileName);
        var text = File.ReadAllText(file, Encoding.UTF8);
        var comment = text.Split('\n').FirstOrDefault(line => line.TrimStart().StartsWith("//")) ?? "";
        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(comment)) builder.AppendLine(comment.TrimEnd());
        builder.AppendLine($"export const {exportName} = [");
        for (var index = 0; index < blocks.Count; index++)
        {
            builder.Append(IndentBlock(blocks[index], 2));
            builder.AppendLine(index == blocks.Count - 1 ? "" : ",");
        }
        builder.AppendLine("];");
        File.WriteAllText(file, builder.ToString(), new UTF8Encoding(false));
    }

    private static string IndentBlock(string block, int spaces)
    {
        var pad = new string(' ', spaces);
        return string.Join(Environment.NewLine, block.Trim().Split('\n').Select(line => pad + line.TrimEnd()));
    }

    private static List<string> ReadArrayBlocks(string fileName) => ReadArrayBlocksFromText(File.ReadAllText(DataPath(fileName), Encoding.UTF8));

    private static List<string> ReadArrayBlocksFromFile(string fileName)
    {
        var text = File.ReadAllText(DataPath(fileName), Encoding.UTF8);
        var itemsMatch = Regex.Match(text, @"items\s*:\s*\[(.*)\]\s*};", RegexOptions.Singleline);
        return itemsMatch.Success ? ExtractObjectBlocks(itemsMatch.Groups[1].Value) : [];
    }

    private static List<string> ReadArrayBlocksFromText(string text)
    {
        var arrayStart = text.IndexOf('[', StringComparison.Ordinal);
        var arrayEnd = text.LastIndexOf("];", StringComparison.Ordinal);
        if (arrayStart < 0 || arrayEnd < arrayStart) throw new InvalidOperationException("Не нашёл массив данных.");
        return ExtractObjectBlocks(text[(arrayStart + 1)..arrayEnd]);
    }

    private static List<string> ExtractObjectBlocks(string body)
    {
        var blocks = new List<string>();
        var inString = false;
        var escaping = false;
        var depth = 0;
        var start = -1;
        for (var index = 0; index < body.Length; index++)
        {
            var symbol = body[index];
            if (inString)
            {
                if (escaping) escaping = false;
                else if (symbol == '\\') escaping = true;
                else if (symbol == '"') inString = false;
                continue;
            }
            if (symbol == '"')
            {
                inString = true;
                continue;
            }
            if (symbol == '{')
            {
                if (depth == 0) start = index;
                depth++;
            }
            else if (symbol == '}')
            {
                depth--;
                if (depth == 0 && start >= 0)
                {
                    blocks.Add(body[start..(index + 1)].Trim());
                    start = -1;
                }
            }
        }
        return blocks;
    }

    private static string ReadStringProperty(string fileName, string property)
    {
        return ReadJsStringProperty(File.ReadAllText(DataPath(fileName), Encoding.UTF8), property);
    }

    private static string ReadJsStringProperty(string block, string property)
    {
        var match = Regex.Match(block, $@"{property}\s*:\s*""((?:\\.|[^""\\])*)""", RegexOptions.Singleline);
        return match.Success ? Unescape(match.Groups[1].Value) : "";
    }

    private static List<string> ReadServices(string block)
    {
        var match = Regex.Match(block, @"services\s*:\s*\[(.*?)\]", RegexOptions.Singleline);
        if (!match.Success) return [];
        return Regex.Matches(match.Groups[1].Value, @"""((?:\\.|[^""\\])*)""")
            .Select(item => Unescape(item.Groups[1].Value))
            .ToList();
    }

    private static void ReplaceStringProperty(string fileName, string property, string value)
    {
        var file = DataPath(fileName);
        var text = File.ReadAllText(file, Encoding.UTF8);
        var next = Regex.Replace(text, $@"({property}\s*:\s*)"".*?""", $"$1\"{Escape(value.Trim())}\"", RegexOptions.Singleline);
        File.WriteAllText(file, next, new UTF8Encoding(false));
    }

    private static (string latitude, string longitude) ReadCoordinates()
    {
        var text = File.ReadAllText(DataPath("company.js"), Encoding.UTF8);
        var match = Regex.Match(text, @"mapCoordinates\s*:\s*\[\s*([0-9.,-]+)\s*,\s*([0-9.,-]+)\s*\]");
        return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : ("", "");
    }

    private static void ReplaceCoordinates(string latitude, string longitude)
    {
        var file = DataPath("company.js");
        var text = File.ReadAllText(file, Encoding.UTF8);
        var next = Regex.Replace(text, @"mapCoordinates\s*:\s*\[\s*[0-9.,-]+\s*,\s*[0-9.,-]+\s*\]", $"mapCoordinates: [{latitude.Trim().Replace(',', '.')}, {longitude.Trim().Replace(',', '.')}]");
        File.WriteAllText(file, next, new UTF8Encoding(false));
    }

    private static void ReplaceTrustTexts(List<string> values)
    {
        var file = DataPath("trust.js");
        var text = File.ReadAllText(file, Encoding.UTF8);
        var index = 0;
        var next = Regex.Replace(text, @"text\s*:\s*""((?:\\.|[^""\\])*)""", match =>
        {
            if (index >= values.Count) return match.Value;
            return $"text: \"{Escape(values[index++].Trim())}\"";
        });
        File.WriteAllText(file, next, new UTF8Encoding(false));
    }

    private static List<string> SplitLines(string value)
    {
        return value.Split(['\n', ','], StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => item.Length > 0)
            .ToList();
    }

    private static void TouchDevServerEntry()
    {
        var file = Path.Combine(ProjectRoot, "src", "index.jsx");
        var text = File.ReadAllText(file, Encoding.UTF8);
        File.WriteAllText(file, text, new UTF8Encoding(false));
    }

    private static string DataPath(string fileName) => Path.Combine(ProjectRoot, "editable-data", fileName);

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n", "\\n");
    }

    private static string Unescape(string value)
    {
        return value.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
    }

    private static string UpperFirst(string value) => string.IsNullOrWhiteSpace(value) ? value : char.ToUpper(value[0]) + value[1..];

    private sealed record Entry(int Index, string Title)
    {
        public override string ToString() => Title;
    }
}
