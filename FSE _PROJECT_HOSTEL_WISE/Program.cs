using Gtk;
using System;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using Cairo;

class WelcomeWindow : Window {
    [Obsolete]
    public WelcomeWindow() : base("Welcome Page") {
        SetDefaultSize(1200, 500);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => Application.Quit();

        Fixed fixedLayout = new Fixed();
        
        try {
            Image background = new Image("home.jpg");
            fixedLayout.Put(background, 0, 0);
        } catch (Exception ex) {
                    Console.WriteLine($"Failed to load background image: {ex.Message}");
                }
        EventBox logoBox = new EventBox();
        
        try {    
            Image logo = new Image("logo.png");
            logoBox.Add(logo);
        fixedLayout.Put(logoBox, 80, 150);
        } catch (Exception ex) {
                    Console.WriteLine($"Failed to load background image: {ex.Message}");
                }
        VBox vbox = new VBox(false, 15)
        {
            BorderWidth = 30
        };

        Button signupBtn = new Button("Sign Up");
        signupBtn.WidthRequest = 150;
        signupBtn.HeightRequest = 40;

        Button loginBtn = new Button("Login");
        loginBtn.WidthRequest = 150;
        loginBtn.HeightRequest = 40;

        Button exitBtn = new Button("🚪 Exit");
        exitBtn.WidthRequest = 120;
        exitBtn.HeightRequest = 40;
        exitBtn.Clicked += (sender, e) => Application.Quit();

        signupBtn.Clicked += (sender, e) =>
        {
            this.Hide(); // ✅ Properly hiding instead of Destroy()
            new SignupWindow().ShowAll();
        };

        loginBtn.Clicked += (sender, e) =>
        {
            this.Hide();
            new LoginWindow().ShowAll();
        };

        HBox buttonBox = new HBox(true, 20);
        buttonBox.PackStart(signupBtn, false, false, 10);
        buttonBox.PackStart(loginBtn, false, false, 10);

        vbox.PackStart(buttonBox, false, false, 100);

        fixedLayout.Put(vbox, 790, 330);
        fixedLayout.Put(exitBtn, 20, 20);

        Add(fixedLayout);
        ShowAll();

    }
}

class BidHostelPage : Window
{
    [Obsolete]
    public BidHostelPage(string hostelId) : base("Hostel Details")
    {
        SetDefaultSize(650, 550);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => this.Hide();

        // ✅ Fixed layout for background layering
        Fixed fixedLayout = new Fixed();

        // ✅ Background image
        try
        {
            Image background = new Image("bidpage.jpg"); // Make sure file is in the correct folder
            fixedLayout.Put(background, 0, 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading background image: " + ex.Message);
        }

        // ✅ Main VBox to hold text and buttons
        VBox mainVBox = new VBox(false, 10) { BorderWidth = 20 };

        // ✅ Fetch hostel details
        var hostel = DatabaseHelper.GetHostelDetailsByReferenceId(hostelId);

        Label titleLabel = new Label($"<span font_desc='20' weight='bold'>{hostel["Name"]}</span>");
        titleLabel.UseMarkup = true;
        titleLabel.SetAlignment(0.5f, 0.5f);

        Label rentLabel = new Label($"<span font_desc='14'>{hostel["Rent"]} PKR</span>");
        rentLabel.UseMarkup = true;
        rentLabel.SetAlignment(0.5f, 0.5f);

        Label locationLabel = new Label($"<span font_desc='14'>{hostel["Location"]}</span>");
        locationLabel.UseMarkup = true;
        locationLabel.SetAlignment(0.5f, 0.5f);

        int rating = Convert.ToInt32(hostel["Ratings"]);
        string stars = new string('⭐', rating);
        Label ratingLabel = new Label($"<span font_desc='14'>{stars}</span>");
        ratingLabel.UseMarkup = true;
        ratingLabel.SetAlignment(0.5f, 0.5f);

        Label seaterLabel = new Label($"<span font_desc='14'>{hostel["Seats"]} Seater(s)</span>");
        seaterLabel.UseMarkup = true;
        seaterLabel.SetAlignment(0.5f, 0.5f);

        Label facilitiesLabel = new Label("<span font_desc='14'><b>Facilities:</b></span>");
        facilitiesLabel.UseMarkup = true;
        facilitiesLabel.SetAlignment(0.5f, 0.5f);

        // 🛠 Facilities line
        string facilities =  $"{(Convert.ToInt32(hostel["Wifi"]) == 1 ? "📶\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Mess"]) == 1 ? "🍽️\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Geyser"]) == 1 ? "🚿\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Cleaning"]) == 1 ? "🧹\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Laundry"]) == 1 ? "🧺\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["SecurityCameras"]) == 1 ? "📹\t\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["AttachedWashroom"]) == 1 ? "🚽\t\t" : "")}";

        Label facilityIcons = new Label(facilities);
        facilityIcons.SetAlignment(0.5f, 0.5f);

        // ✅ Contact Button
        Button contactBtn = new Button("📲 Contact Owner");
        contactBtn.SetSizeRequest(100, 40);
        contactBtn.Clicked += (s, e) =>
        {
            string? whatsapp = hostel.ContainsKey("WhatsApp") ? hostel["WhatsApp"].ToString() : "";
            if (!string.IsNullOrEmpty(whatsapp))
            {
                string link = $"https://wa.me/{whatsapp}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
        };

        Button locationBtn = new Button("📍 See Location");
        locationBtn.SetSizeRequest(100, 40);
        locationBtn.Clicked += (s, e) =>
{
    string? location = hostel.ContainsKey("Location") ? hostel["Location"].ToString() : "";
    if (!string.IsNullOrEmpty(location))
    {
        // Encode location string for URL safety
        string encodedLocation = Uri.EscapeDataString(location);
        string mapsLink = $"https://www.google.com/maps/search/?api=1&query={encodedLocation}";

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = mapsLink,
            UseShellExecute = true
        });
    }
};

        // ✅ Pack all into VBox
        mainVBox.PackStart(titleLabel, false, false, 15);
        mainVBox.PackStart(rentLabel, false, false, 5);
        mainVBox.PackStart(locationLabel, false, false, 5);
        mainVBox.PackStart(ratingLabel, false, false, 5);
        mainVBox.PackStart(seaterLabel, false, false, 5);
        mainVBox.PackStart(facilitiesLabel, false, false, 5);
        mainVBox.PackStart(facilityIcons, false, false, 5);
        mainVBox.PackStart(contactBtn, false, false, 5);
        mainVBox.PackStart(locationBtn, false, false, 5);

        // ✅ Add VBox on top of background
        fixedLayout.Put(mainVBox, 85, 90); // Adjust X/Y for center alignment

        // ✅ Add to window
        Add(fixedLayout);
        ShowAll();
    }
}





class MyHostel : Window
{
    private VBox hostelListVBox;
    private int currentUserId;

    [Obsolete]
    public MyHostel(int userId) : base("My Hostel")
    {
        currentUserId = userId;

        SetDefaultSize(800, 600);
        SetPosition(WindowPosition.Center);
        ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

        VBox mainVBox = new VBox(false, 10);

        Label titleLabel = new Label();
        titleLabel.Markup = "<span font='22'><b>My Hostel Dashboard</b></span>";
        titleLabel.SetAlignment(0.5f, 0.5f);
        mainVBox.PackStart(titleLabel, false, false, 15);

        ScrolledWindow scroll = new ScrolledWindow();
        scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        hostelListVBox = new VBox(false, 15);

        scroll.AddWithViewport(hostelListVBox);
        mainVBox.PackStart(scroll, true, true, 0);

        Add(mainVBox);
        ShowAll();

        LoadHostelCards();
    }

    [Obsolete]
    private void LoadHostelCards()
    {
        hostelListVBox.Foreach(widget => hostelListVBox.Remove(widget));
        hostelListVBox.ShowAll();

        var hostels = DatabaseHelper.GetHostelsByUserId(currentUserId);

        foreach (var hostel in hostels)
        {
            Frame card = new Frame();
            card.ModifyBg(StateType.Normal, new Gdk.Color(50, 50, 50));

            VBox cardVBox = new VBox(false, 8);
            card.BorderWidth = 10;
            int rating = Convert.ToInt32(hostel["Ratings"]);
            string stars = new string('⭐', rating);
        
            string details = $"<b>Name:</b> \t   {hostel["Name"]}\n" +
                 $"<b>Rent:</b> \t   {hostel["Rent"]} PKR\n" +
                 $"<b>Location:</b>   {hostel["Location"]}\n" +
                 $"<b>Rating:</b> \t   {stars}\n" +
                 $"<b>Seater:</b> \t   {hostel["Seats"]}\n\n" +
                 $"<b>Facilities:</b>\n\n" +
                 $"{(Convert.ToInt32(hostel["Wifi"]) == 1 ? "📶\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["Mess"]) == 1 ? "🍽️\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["Geyser"]) == 1 ? "🚿\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["Cleaning"]) == 1 ? "🧹\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["Laundry"]) == 1 ? "🧺\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["SecurityCameras"]) == 1 ? "📹\t" : "")}" +
                 $"{(Convert.ToInt32(hostel["AttachedWashroom"]) == 1 ? "🚽" : "")}";

            Label infoLabel = new Label();
            infoLabel.Markup = $"<span font='11.5' foreground='white'>{details}</span>";
            infoLabel.Xalign = 0;
            cardVBox.PackStart(infoLabel, false, false, 5);

            HBox buttonBox = new HBox(true, 10);
            Button editBtn = new Button("✏️ Edit");
            Button removeBtn = new Button("🗑️ Remove");

            editBtn.Clicked += (s, e) =>
            {
                Dialog editDialog = new Dialog("Edit Hostel Info", this, DialogFlags.Modal);
                editDialog.SetDefaultSize(650, 550);
                editDialog.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0, 0, 0));

                VBox editVBox = new VBox(false, 10);
                Entry nameEntry = new Entry { Text = hostel["Name"].ToString() };
                Entry rentEntry = new Entry { Text = hostel["Rent"].ToString() };
                Entry locationEntry = new Entry { Text = hostel["Location"].ToString() };
                Entry seaterEntry = new Entry { Text = hostel["Seats"].ToString() };
                Entry ratingEntry = new Entry { Text = hostel["Ratings"].ToString() };
                Entry whatsappEntry = new Entry {
                    Text = hostel.ContainsKey("WhatsApp") ? hostel["WhatsApp"].ToString() : ""
                };

                editVBox.PackStart(new Label("Name:"), false, false, 5);
                editVBox.PackStart(nameEntry, false, false, 5);
                editVBox.PackStart(new Label("Rent (PKR):"), false, false, 5);
                editVBox.PackStart(rentEntry, false, false, 5);
                editVBox.PackStart(new Label("Location:"), false, false, 5);
                editVBox.PackStart(locationEntry, false, false, 5);
                editVBox.PackStart(new Label("Seater:"), false, false, 5);
                editVBox.PackStart(seaterEntry, false, false, 5);
                editVBox.PackStart(new Label("Rating (1-5):"), false, false, 5);
                editVBox.PackStart(ratingEntry, false, false, 5);
                editVBox.PackStart(new Label("WhatsApp (+92):"), false, false, 5);
                editVBox.PackStart(whatsappEntry, false, false, 5);

                editDialog.AddButton("Cancel", ResponseType.Cancel);
                Button saveButton = new Button("Save Changes");
                editDialog.ActionArea.PackStart(saveButton, false, false, 5);
                editDialog.ContentArea.PackStart(editVBox, true, true, 5);
                editDialog.ShowAll();

                saveButton.Clicked += (sender, args) =>
                {
                    string updatedName = nameEntry?.Text ?? "";
                    string updatedRent = rentEntry?.Text ?? "";
                    string updatedLocation = locationEntry?.Text ?? "";
                    string updatedSeater = seaterEntry?.Text ?? "";
                    string updatedRating = ratingEntry?.Text ?? "";
                    string updatedWhatsapp = whatsappEntry?.Text ?? "";

        #pragma warning disable CS8604
                            bool success = DatabaseHelper.UpdateHostelInfo(
                                hostel["ReferenceID"].ToString(),
                                updatedName,
                                updatedRent,
                                updatedLocation,
                                updatedSeater,
                                updatedRating,
                                updatedWhatsapp
                            );
        #pragma warning restore CS8604

                    if (success)
                    {
                        MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                            "Hostel info updated successfully!");
                        dialog.Run(); dialog.Destroy();
                        editDialog.Destroy();
                        LoadHostelCards();
                    }
                    else
                    {
                        MessageDialog error = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                            "Failed to update hostel info.");
                        error.Run(); error.Destroy();
                    }
                };

                editDialog.Run();
                editDialog.Destroy();
            };

            removeBtn.Clicked += (s, e) =>
            {
                MessageDialog confirm = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo,
                    "Are you sure you want to remove this hostel?");
                if ((ResponseType)confirm.Run() == ResponseType.Yes)
                {
                    confirm.Destroy();
        #pragma warning disable CS8600 
                            string refId = hostel["ReferenceID"].ToString();
#pragma warning restore CS8600
#pragma warning disable CS8604
                    bool success = DatabaseHelper.RemoveHostel(refId);
#pragma warning restore CS8604

                    if (success)
                    {
                        MessageDialog info = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                            "Hostel removed successfully.");
                        info.Run(); info.Destroy();
                        LoadHostelCards();
                    }
                    else
                    {
                        MessageDialog error = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                            "Failed to remove hostel.");
                        error.Run(); error.Destroy();
                    }
                }
                else confirm.Destroy();
            };

            buttonBox.PackStart(editBtn, true, true, 0);
            buttonBox.PackStart(removeBtn, true, true, 0);
            cardVBox.PackStart(buttonBox, false, false, 5);
            card.Add(cardVBox);
            hostelListVBox.PackStart(card, false, false, 10);
        }

        hostelListVBox.ShowAll();
    }
}

class ExploreHostels : Window
{
    private VBox hostelListVBox;
    private ComboBoxText sortCombo = new ComboBoxText(); // ✅ Yeh sahi hai

    private Entry seaterEntry;
    private List<Dictionary<string, object>> allHostels;

    [Obsolete]
    public ExploreHostels() : base("Explore Hostels")
    {
        SetDefaultSize(850, 600);
        SetPosition(WindowPosition.Center);
        ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));  // Black background

        VBox mainVBox = new VBox(false, 10);

        // 🟢 Title
        Label titleLabel = new Label();
        titleLabel.Markup = "<span font='22'><b>🌐 All Sponsored Hostels</b></span>";
        titleLabel.SetAlignment(0.5f, 0.5f);
        mainVBox.PackStart(titleLabel, false, false, 15);

        // 🔽 Filters Section ComboBox sortCombo
        HBox filtersBox = new HBox(false, 10) { BorderWidth = 10 };

        sortCombo = new ComboBoxText();
        sortCombo.AppendText("Sort By");
        sortCombo.AppendText("Price Low-High");
        sortCombo.AppendText("Price High-Low");
        sortCombo.AppendText("Rating High-Low");
        sortCombo.AppendText("Rating Low-High");
        sortCombo.Active = 0; // Default selected

        filtersBox.PackStart(sortCombo, false, false, 5);

        seaterEntry = new Entry();
        seaterEntry.PlaceholderText = "Enter Seater";
        filtersBox.PackStart(seaterEntry, false, false, 5);

        Button applyBtn = new Button("Apply Filters");
        applyBtn.Clicked += (s, e) => LoadHostelCards();
        filtersBox.PackStart(applyBtn, false, false, 5);

        mainVBox.PackStart(filtersBox, false, false, 5);

        // 🔁 Scrollable Area
        ScrolledWindow scroll = new ScrolledWindow();
        scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        hostelListVBox = new VBox(false, 15);
        scroll.AddWithViewport(hostelListVBox);
        mainVBox.PackStart(scroll, true, true, 0);

        Button exportBtn = new Button("📤 Export Hostel Data to CSV File");
        #pragma warning disable CS8604 
        exportBtn.Clicked += (s, e) => ExportToCSV(allHostels); // allHostels tumhare paas already hai
        #pragma warning restore CS8604 
        mainVBox.PackStart(exportBtn, false, false, 10);

        Add(mainVBox);

        // 📥 Get Hostel Data Once
        allHostels = DatabaseHelper.GetAllHostels();
        LoadHostelCards();

        ShowAll();
    }

    [Obsolete]
    private void LoadHostelCards()
    {
        hostelListVBox.Foreach(child => hostelListVBox.Remove(child));

        var hostels = new List<Dictionary<string, object>>(allHostels);

        // 🧠 Sorting
        string selectedSort = sortCombo.ActiveText;
        if (selectedSort == "Price Low-High")
            hostels = hostels.OrderBy(h => Convert.ToInt32(h["Rent"])).ToList();
        else if (selectedSort == "Price High-Low")
            hostels = hostels.OrderByDescending(h => Convert.ToInt32(h["Rent"])).ToList();
        else if (selectedSort == "Rating High-Low")
            hostels = hostels.OrderByDescending(h => Convert.ToDouble(h["Ratings"])).ToList();
        else if (selectedSort == "Rating Low-High")
            hostels = hostels.OrderBy(h => Convert.ToDouble(h["Ratings"])).ToList();

        // 🧪 Seater Filter sortCombo = new ComboBox(new string[]
        if (!string.IsNullOrWhiteSpace(seaterEntry.Text))
        {
            if (int.TryParse(seaterEntry.Text, out int seater))
                hostels = hostels.Where(h => Convert.ToInt32(h["Seats"]) == seater).ToList();
        }

        // 🔁 Hostel Cards
        foreach (var hostel in hostels)
        {
            Frame card = new Frame();
            card.ModifyBg(StateType.Normal, new Gdk.Color(50, 50, 50));

            VBox cardVBox = new VBox(false, 8);
            card.BorderWidth = 10;
        
            int rating = Convert.ToInt32(hostel["Ratings"]);
            string stars = new string('⭐', rating);
        
            string details = $"<b>Name:</b> \t   {hostel["Name"]}\n" +
                             $"<b>Rent:</b> \t   {hostel["Rent"]} PKR\n" +
                             $"<b>Location:</b>   {hostel["Location"]}\n" +
                             $"<b>Rating:</b> \t   {stars}\n" +
                             $"<b>Seater:</b> \t   {hostel["Seats"]}\n\n" +
                             $"<b>Facalities:</b> \n\n" +
                             $"{(Convert.ToInt32(hostel["Wifi"]) == 1 ? "📶\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Mess"]) == 1 ? "🍽️\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Geyser"]) == 1 ? "🚿\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Cleaning"]) == 1 ? "🧹\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["Laundry"]) == 1 ? "🧺\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["SecurityCameras"]) == 1 ? "📹\t" : "")}" +
                             $"{(Convert.ToInt32(hostel["AttachedWashroom"]) == 1 ? "🚽" : "")}";

            Label infoLabel = new Label();
            infoLabel.Markup = $"<span font='11.5' foreground='white'>{details}</span>";
            infoLabel.Xalign = 0;
            cardVBox.PackStart(infoLabel, false, false, 5);

            HBox buttonBox = new HBox(true, 10);
            Button bidBtn = new Button("🔎  See More Details");
            bidBtn.Clicked += (s, e) =>
            {
#pragma warning disable CS8600
                string selectedReferenceId = hostel["ReferenceID"].ToString();
#pragma warning restore CS8600
#pragma warning disable CS8604
                new BidHostelPage(selectedReferenceId).ShowAll();
#pragma warning restore CS8604
            };

            buttonBox.PackStart(bidBtn, true, true, 0);
            cardVBox.PackStart(buttonBox, false, false, 5);

            card.Add(cardVBox);
            hostelListVBox.PackStart(card, false, false, 10);
        }

        hostelListVBox.ShowAll();
    }


    private void ExportToCSV(List<Dictionary<string, object>> hostels)
{
    string filePath = System.IO.Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads",
    "HostelsExport.csv"
    );
    using (StreamWriter writer = new StreamWriter(filePath))
    {
        // Header
        writer.WriteLine("Name,Rent,Location,Rating,Seats,Wifi,Mess,Geyser,Cleaning,Laundry,SecurityCameras,AttachedWashroom");

        // Data rows
        foreach (var hostel in hostels)
        {
            string row = $"{hostel["Name"]}," +
                         $"{hostel["Rent"]}," +
                         $"{hostel["Location"]}," +
                         $"{hostel["Ratings"]}," +
                         $"{hostel["Seats"]}," +
                         $"{hostel["Wifi"]}," +
                         $"{hostel["Mess"]}," +
                         $"{hostel["Geyser"]}," +
                         $"{hostel["Cleaning"]}," +
                         $"{hostel["Laundry"]}," +
                         $"{hostel["SecurityCameras"]}," +
                         $"{hostel["AttachedWashroom"]}";
            writer.WriteLine(row);
        }
    }

    MessageDialog successDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, 
        "✅ CSV exported successfully to Desktop!");
    successDialog.Run();
    successDialog.Destroy();
}


}

class ProfileWindow : Window
{
    private string userEmail;

    [Obsolete]
    public ProfileWindow(string fullName, string email, string phone, string city, string age, string gender) : base("Profile")
    {
        userEmail = email;
        SetDefaultSize(650, 550);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => this.Hide();

        Fixed fixedLayout = new Fixed();

        try {
            Image background = new Image("profile.jpg");
            fixedLayout.Put(background, 0, 0);
        } catch (Exception ex) {
            Console.WriteLine($"Failed to load background image: {ex.Message}");
        }

        VBox vbox = new VBox(false, 10) { BorderWidth = 20 };

        Label titleLabel = new Label($"<span font_desc='35' weight='bold'>{fullName}</span>");
        titleLabel.UseMarkup = true;
        titleLabel.SetAlignment(0.5f, 0.5f);

        Label nameLabel = new Label($"<span font_desc='15'>Name: {fullName}</span>") { UseMarkup = true };
        Label emailLabel = new Label($"<span font_desc='15'>Email: {email}</span>") { UseMarkup = true };
        Label phoneLabel = new Label($"<span font_desc='15'>Phone: {phone}</span>") { UseMarkup = true };
        Label cityLabel = new Label($"<span font_desc='15'>City: {city}</span>") { UseMarkup = true };
        Label ageLabel = new Label($"<span font_desc='15'>Age: {age}</span>") { UseMarkup = true };
        Label genderLabel = new Label($"<span font_desc='15'>Gender: {gender}</span>") { UseMarkup = true };

        Button editButton = new Button("Edit Profile");

        editButton.Clicked += (sender, e) =>
        {
            Dialog editDialog = new Dialog("Edit Profile", this, DialogFlags.Modal);
            editDialog.SetDefaultSize(650, 550);
            editDialog.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));  // Set background to black

            VBox editBox = new VBox(false, 10); // Create a new VBox, which will be added to the dialog's content area
            editDialog.ContentArea.Add(editBox); // Add it to the dialog's content area

            Entry nameEntry = new Entry { Text = fullName };
            Entry phoneEntry = new Entry { Text = phone };
            Entry cityEntry = new Entry { Text = city };
            Entry ageEntry = new Entry { Text = age };
            Entry genderEntry = new Entry { Text = gender };

            editBox.PackStart(new Label("Full Name:"), false, false, 5);
            editBox.PackStart(nameEntry, false, false, 5);
            editBox.PackStart(new Label("Phone:"), false, false, 5);
            editBox.PackStart(phoneEntry, false, false, 5);
            editBox.PackStart(new Label("City:"), false, false, 5);
            editBox.PackStart(cityEntry, false, false, 5);
            editBox.PackStart(new Label("Age:"), false, false, 5);
            editBox.PackStart(ageEntry, false, false, 5);
            editBox.PackStart(new Label("Gender:"), false, false, 5);
            editBox.PackStart(genderEntry, false, false, 5);

            editDialog.AddButton("Cancel", ResponseType.Cancel);
            editDialog.AddButton("Update", ResponseType.Ok);

            editDialog.ShowAll();

            editDialog.Response += (o, responseArgs) =>
            {
                if (responseArgs.ResponseId == ResponseType.Ok)
                {
                    string newName = nameEntry.Text;
                    string newPhone = phoneEntry.Text;
                    string newCity = cityEntry.Text;
                    string newAge = ageEntry.Text;
                    string newGender = genderEntry.Text;

                    bool success = DatabaseHelper.UpdateUserProfile(userEmail, newName, newPhone, newCity, newAge, newGender);

                    if (success)
                    {
                        MessageDialog successMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "🎉  Profile Updated Successfully!");
                        successMd.Run();
                        successMd.Destroy();
                        this.Destroy(); // Close old window
                        new ProfileWindow(newName, userEmail, newPhone, newCity, newAge, newGender).ShowAll(); // Open updated profile
                    }
                    else
                    {
                        MessageDialog errorMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "❌  Failed to update profile!");
                        errorMd.Run();
                        errorMd.Destroy();
                    }
                }
                editDialog.Destroy();
            };
        };

        vbox.PackStart(titleLabel, false, false, 30);
        vbox.PackStart(nameLabel, false, false, 5);
        vbox.PackStart(emailLabel, false, false, 5);
        vbox.PackStart(phoneLabel, false, false, 5);
        vbox.PackStart(cityLabel, false, false, 5);
        vbox.PackStart(ageLabel, false, false, 5);
        vbox.PackStart(genderLabel, false, false, 5);
        vbox.PackStart(editButton, false, false, 30);

        fixedLayout.Put(vbox, 100, 60);

        Add(fixedLayout);
        ShowAll();
    }
}


class SettingsWindow : Window
{
    [Obsolete]
    public SettingsWindow() : base("Settings")
    {
        SetDefaultSize(650, 550);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => this.Hide();

        // Use Fixed layout for background image
        Fixed fixedLayout = new Fixed();

        // Set background image (home.jpg)
        try
        {
            Image background = new Image("settings.jpg");
            fixedLayout.Put(background, 0, 0); // Place at top-left
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load background image: {ex.Message}");
        }

        // VBox for content
        VBox vbox = new VBox(false, 15)
        {
            BorderWidth = 30 // Add some padding like LoginWindow
        };

        // Title label styled like LoginWindow
        Label passwordforgot = new Label();
        passwordforgot.Markup = "<span font_desc='15' weight='bold'>Forget Password</span>";
        passwordforgot.SetAlignment(0.5f, 0.5f);

        // Forget Password Button with fixed size
        Button forgetPasswordBtn = new Button("🔑 Forget Password");
        forgetPasswordBtn.WidthRequest = 320;  // Fixed width like LoginWindow
        forgetPasswordBtn.HeightRequest = 40;  // Fixed height like LoginWindow
        #pragma warning disable CS8622
        forgetPasswordBtn.Clicked += OnForgetPasswordClicked;
        #pragma warning restore CS8622

        Label securityquesiton = new Label();
        securityquesiton.Markup = "<span font_desc='15' weight='bold'> Update Security Question</span>";
        securityquesiton.SetAlignment(0.5f, 0.5f);

        // Update Security Question Button with fixed size
        Button updateSecurityQuestionBtn = new Button("❓ Update Security Question");
        updateSecurityQuestionBtn.WidthRequest = 320;  // Fixed width
        updateSecurityQuestionBtn.HeightRequest = 40;  // Fixed height
        #pragma warning disable CS8622 
        updateSecurityQuestionBtn.Clicked += OnUpdateSecurityQuestionClicked;
        #pragma warning restore CS8622

        // Pack widgets into VBox
        vbox.PackStart(passwordforgot, false, false, 15);
        vbox.PackStart(forgetPasswordBtn, false, false, 10);
        vbox.PackStart(securityquesiton, false, false, 15);
        vbox.PackStart(updateSecurityQuestionBtn, false, false, 10);

        // Position the VBox in the Fixed layout
        fixedLayout.Put(vbox, 80, 140); // Adjusted position similar to LoginWindow

        Add(fixedLayout);
        ShowAll();
    }

    private void OnForgetPasswordClicked(object sender, EventArgs e)
    {
        Dialog dialog = new Dialog("Forget Password", this, DialogFlags.Modal);
        dialog.SetDefaultSize(500, 400);
        
        var provider = new CssProvider();
        provider.LoadFromData("dialog { background-color: black; } label { color: white; }");
        dialog.StyleContext.AddProvider(provider, StyleProviderPriority.User);
        
        var contentArea = dialog.ContentArea;
        Entry emailEntry = new Entry { PlaceholderText = "Enter your email" };
        Entry securityQuestionEntry = new Entry { PlaceholderText = "Enter your security answer" };
        Entry newPasswordEntry = new Entry { PlaceholderText = "Enter new password", Visibility = false };
        CheckButton showPasswordCheck = new CheckButton("Show Password");
        showPasswordCheck.Toggled += (s, ev) => newPasswordEntry.Visibility = showPasswordCheck.Active;
        
        contentArea.PackStart(new Label("Enter details to reset your password:"), false, false, 10);
        contentArea.PackStart(emailEntry, false, false, 5);
        contentArea.PackStart(securityQuestionEntry, false, false, 5);
        contentArea.PackStart(newPasswordEntry, false, false, 5);
        contentArea.PackStart(showPasswordCheck, false, false, 5);
        
        dialog.AddButton("Cancel", ResponseType.Cancel);
        dialog.AddButton("Reset Password", ResponseType.Ok);
        dialog.ShowAll();
        
        if (dialog.Run() == (int)ResponseType.Ok)
        {
            if (DatabaseHelper.ResetPassword(emailEntry.Text, securityQuestionEntry.Text, newPasswordEntry.Text))
            {
                MessageDialog success = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "🎉 Password Reset Successful!");
                success.Run();
                success.Destroy();
            }
            else
            {
                MessageDialog error = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Invalid details!");
                error.Run();
                error.Destroy();
            }
        }
        dialog.Destroy();
    }

    private void OnUpdateSecurityQuestionClicked(object sender, EventArgs e)
    {
        Dialog dialog = new Dialog("Update Security Question", this, DialogFlags.Modal);
        dialog.SetDefaultSize(500, 400);
        
        var provider = new CssProvider();
        provider.LoadFromData("dialog { background-color: black; } label { color: white; }");
        dialog.StyleContext.AddProvider(provider, StyleProviderPriority.User);
        
        var contentArea = dialog.ContentArea;
        Entry emailEntry = new Entry { PlaceholderText = "Enter your email" };
        Entry passwordEntry = new Entry { PlaceholderText = "Enter your password", Visibility = false };
        Entry newSecurityQuestionEntry = new Entry { PlaceholderText = "Enter new security question" };
        CheckButton showPasswordCheck = new CheckButton("Show Password");
        showPasswordCheck.Toggled += (s, ev) => passwordEntry.Visibility = showPasswordCheck.Active;
        
        contentArea.PackStart(new Label("Enter details to update security question:"), false, false, 10);
        contentArea.PackStart(emailEntry, false, false, 5);
        contentArea.PackStart(passwordEntry, false, false, 5);
        contentArea.PackStart(showPasswordCheck, false, false, 5);
        contentArea.PackStart(newSecurityQuestionEntry, false, false, 5);
        
        dialog.AddButton("Cancel", ResponseType.Cancel);
        dialog.AddButton("Update", ResponseType.Ok);
        dialog.ShowAll();
        
        if (dialog.Run() == (int)ResponseType.Ok)
        {
            if (DatabaseHelper.UpdateSecurityQuestion(emailEntry.Text, passwordEntry.Text, newSecurityQuestionEntry.Text))
            {
                MessageDialog success = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "🎉 Security Question Updated Successfully!");
                success.Run();
                success.Destroy();
            }
            else
            {
                MessageDialog error = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Invalid details!");
                error.Run();
                error.Destroy();
            }
        }
        dialog.Destroy();
    }
}

class PostHostles : Window
{
    private int currentUserId;

    [Obsolete]
    public PostHostles(int userId) : base("Post Hostel - Hostel Wise")
    {   
        this.currentUserId = userId;
        SetDefaultSize(900, 600); // Increased the window size for better fit
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => this.Hide();
        this.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

        VBox mainLayout = new VBox(false, 10) { BorderWidth = 15 };

        Label title = new Label("<b><big>Enter Your Hostel Information</big></b>");
        title.UseMarkup = true;
        mainLayout.PackStart(title, false, false, 10);

        Table form = new Table(11, 2, false); // Increased row count
        form.RowSpacing = 10; // Increased row spacing
        form.ColumnSpacing = 15; // Increased column spacing

        // Text Entries
        Entry sponsorEntry = new Entry() { PlaceholderText = "Sponsor Name" };
        Entry referenceIdEntry = new Entry() { PlaceholderText = "Auto-generated Reference ID" };
        referenceIdEntry.Text = Guid.NewGuid().ToString(); // Auto-generate unique reference ID
        referenceIdEntry.Sensitive = false; // User cannot edit it
        Entry nameEntry = new Entry() { PlaceholderText = "Hostel Name" };
        Entry locationEntry = new Entry() { PlaceholderText = "Location" };
        Entry rentEntry = new Entry() { PlaceholderText = "Rent (PKR)" };
        Entry ratingEntry = new Entry() { PlaceholderText = "Rating (1-5)" };
        Entry seatsEntry = new Entry() { PlaceholderText = "Seats (e.g. 4)" };
        Entry whatsappEntry = new Entry() { PlaceholderText = "WhatsApp No. (+92 format)" };

        // Toggle buttons (CheckButtons)
        CheckButton wifiToggle = new CheckButton("Wifi");
        CheckButton laundryToggle = new CheckButton("Laundry");
        CheckButton cleaningToggle = new CheckButton("Cleaning");
        CheckButton attachedWashroomToggle = new CheckButton("Attached Washroom");
        CheckButton securityToggle = new CheckButton("Security Cameras");
        CheckButton geyserToggle = new CheckButton("Geyser");
        CheckButton messToggle = new CheckButton("Mess");

        // Adding widgets to form
        form.Attach(new Label("Sponsor Name:"), 0, 1, 0, 1);
        form.Attach(sponsorEntry, 1, 2, 0, 1);

        form.Attach(new Label("Hostel Name:"), 0, 1, 1, 2);
        form.Attach(nameEntry, 1, 2, 1, 2);

        form.Attach(new Label("Location:"), 0, 1, 2, 3);
        form.Attach(locationEntry, 1, 2, 2, 3);

        form.Attach(new Label("Rent:"), 0, 1, 3, 4);
        form.Attach(rentEntry, 1, 2, 3, 4);

        form.Attach(new Label("Rating:"), 0, 1, 4, 5);
        form.Attach(ratingEntry, 1, 2, 4, 5);

        form.Attach(new Label("Seats:"), 0, 1, 5, 6);
        form.Attach(seatsEntry, 1, 2, 5, 6);

        form.Attach(new Label("WhatsApp No.:"), 0, 1, 6, 7);
        form.Attach(whatsappEntry, 1, 2, 6, 7);

        form.Attach(wifiToggle, 0, 1, 7, 8);
        form.Attach(laundryToggle, 1, 2, 7, 8);

        form.Attach(cleaningToggle, 0, 1, 8, 9);
        form.Attach(attachedWashroomToggle, 1, 2, 8, 9);

        form.Attach(securityToggle, 0, 1, 9, 10);
        form.Attach(geyserToggle, 1, 2, 9, 10);

        form.Attach(messToggle, 0, 1, 10, 11);

        // Submit Button
        Button submitButton = new Button("Submit Hostel Info");
        submitButton.Clicked += (sender, e) =>
        {
            try
            {
                int rent = int.Parse(rentEntry.Text);
                int rating = int.Parse(ratingEntry.Text);
                int seats = int.Parse(seatsEntry.Text);

                bool inserted = DatabaseHelper.InsertHostel(
                        currentUserId,
                        referenceIdEntry.Text,
                        nameEntry.Text,
                        locationEntry.Text,
                        rent,
                        rating,
                        seats,
                        wifiToggle.Active,
                        laundryToggle.Active,
                        cleaningToggle.Active,
                        attachedWashroomToggle.Active,
                        securityToggle.Active,
                        geyserToggle.Active,
                        messToggle.Active,
                        whatsappEntry.Text
                );

                if (inserted)
                {
                    MessageDialog successDialog = new MessageDialog(this, 
                        DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, 
                        "🎉 Hostel posted successfully!");
                    successDialog.Run();
                    successDialog.Destroy();
                }
                else
                {
                    MessageDialog errorDialog = new MessageDialog(this,
                        DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                        "❌ Failed to post hostel. Please try again.");
                    errorDialog.Run();
                    errorDialog.Destroy();
                }
            }
            catch (FormatException)
            {
                MessageDialog errorDialog = new MessageDialog(this,
                    DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    "⚠️ Please enter valid numeric values for Rent, Rating, and Seats.");
                errorDialog.Run();
                errorDialog.Destroy();
            }
        };

        mainLayout.PackStart(form, false, false, 10);
        // Submit and Cancel buttons together
        HBox buttonBox = new HBox(true, 10); // Horizontal box with spacing 10
        buttonBox.BorderWidth = 10;

        // Cancel Button
        Button cancelButton = new Button("Cancel");
        cancelButton.Clicked += (sender, e) => this.Destroy();

        // Add both buttons to HBox
        buttonBox.PackStart(submitButton, true, true, 0);
        buttonBox.PackStart(cancelButton, true, true, 0);

        // Add HBox to main layout
        mainLayout.PackStart(buttonBox, false, false, 15);


        Add(mainLayout);
        ShowAll();
    }
}

public class Contact : Window
{
    [Obsolete]
    public Contact() : base("Contact Us")
    {
        SetDefaultSize(650, 550);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => this.Hide();

        // ✅ Fixed layout for background layering
        Fixed fixedLayout = new Fixed();

        // ✅ Background image
        try
        {
            Image background = new Image("contacus.jpg"); // Make sure the image file is in the correct folder
            fixedLayout.Put(background, 0, 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading background image: " + ex.Message);
        }

        // ✅ Main VBox to hold text and buttons
        VBox mainVBox = new VBox(false, 10) { BorderWidth = 20 };

        // Space in the middle
        Label spacer = new Label();
        spacer.Text = "\n\n";
        mainVBox.PackStart(spacer, false, false, 10);

        // Button Row (Using Box for horizontal layout)
        Box buttonRow = new Box(Orientation.Horizontal, 10); // HBox equivalent

        Button contact1 = new Button("CONTACT HIM");
        contact1.Clicked += delegate {
            OpenWhatsApp("+923263355747");
        };

        Button contact2 = new Button("CONTACT HIM");
        contact2.Clicked += delegate {
            OpenWhatsApp("+923341459040");
        };

        Button contact3 = new Button("CONTACT HIM");
        contact3.Clicked += delegate {
            OpenWhatsApp("+923048604324");
        };

        buttonRow.PackStart(contact1, true, true, 50);  // Increased padding to 20
        buttonRow.PackStart(contact2, true, true, 50);  // Increased padding to 20
        buttonRow.PackStart(contact3, true, true, 50);

        mainVBox.PackEnd(buttonRow, false, false, 10);

        // ✅ Pack everything into the fixed layout
        fixedLayout.Put(mainVBox, 30, 410); // Adjust the X/Y position for center alignment

        // ✅ Add everything to the window
        Add(fixedLayout);
        ShowAll();
    }

    private void OpenWhatsApp(string number)
    {
        string url = "https://wa.me/" + number.Replace("+", "");
        try
        {
            System.Diagnostics.Process.Start("xdg-open", url); // Linux-specific
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening browser: " + ex.Message);
        }
    }
}



class DashboardWindow : Window
{
    private string userEmail;

    [Obsolete]
    public DashboardWindow(string userName, string email) : base("Dashboard - Hostle Wise")
    {
        userEmail = email;

        SetDefaultSize(1200, 600);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => Application.Quit();

        Fixed fixedLayout = new Fixed();
        try {
            Image background = new Image("dashboard.jpg");
            fixedLayout.Put(background, 0, 0);
            } catch (Exception ex) {
                        Console.WriteLine($"Failed to load background image: {ex.Message}");
                    }
        // Sidebar
        VBox sidebar = new VBox(false, 10)
        {
            BorderWidth = 10
        };
        sidebar.WidthRequest = 200;

        Label dashboardLabel = new Label("<span font_desc='18' weight='bold'>Dashboard</span>")
        {
            UseMarkup = true
        };
        sidebar.PackStart(dashboardLabel, false, false, 10);

        Button ExplorehostelsBtn = new Button("🔍 Explore Hostels");
        Button posthostlesBtn = new Button("➕ Post Hostles");
        Button myhostelsBtn = new Button ("🏠 My Hostels");
        Button profileBtn = new Button("👤 Profile");
        Button settingsBtn = new Button("⚙️ Settings");
        Button logoutBtn = new Button("🚪 Logout");
        Button exitBtn = new Button("❌ Exit");

        // Contact Form Layout
VBox formLayout = new VBox(false, 10);
formLayout.BorderWidth = 20;

// Contact Us Button
Button contactUsBtn = new Button("📞 Contact Us");
fixedLayout.Put(contactUsBtn, 490, 475);

// Name Entry
Label nameLabel = new Label();
nameLabel.UseMarkup = true;
nameLabel.Markup = "<b>Your Name:</b>";

Entry nameEntry = new Entry();
formLayout.PackStart(nameLabel, false, false, 2);
formLayout.PackStart(nameEntry, false, false, 5);

// Email Entry
Label emailLabel = new Label();
emailLabel.UseMarkup = true;
emailLabel.Markup = "<b>Your Email:</b>";

Entry emailEntry = new Entry();
formLayout.PackStart(emailLabel, false, false, 2);
formLayout.PackStart(emailEntry, false, false, 5);

// Message TextView
Label messageBoxLabel = new Label();
messageBoxLabel.UseMarkup = true;
messageBoxLabel.Markup = "<b>Your Message:</b>";

TextView messageBox = new TextView();
messageBox.SetSizeRequest(300, 150);

formLayout.PackStart(messageBoxLabel, false, false, 2);
formLayout.PackStart(messageBox, false, false, 5);

// Submit Button
Button submitBtn = new Button("Submit");
formLayout.PackStart(submitBtn, false, false, 10);

// Final placement on FixedLayout — right side of the sidebar
fixedLayout.Put(formLayout, 910, 270);  // Adjust this X & Y if needed


        sidebar.PackStart(ExplorehostelsBtn, false, false, 10);
        sidebar.PackStart(posthostlesBtn, false, false, 10);
        sidebar.PackStart(myhostelsBtn, false, false, 10);
        sidebar.PackStart(profileBtn, false, false, 10);
        sidebar.PackStart(settingsBtn, false, false, 10);
        sidebar.PackStart(logoutBtn, false, false, 10);
        sidebar.PackStart(exitBtn, false, false, 70);

        fixedLayout.Put(sidebar, 10, 50);

        // Welcome Label
        Label welcomeLabel = new Label();
        welcomeLabel.Markup = $"<span font_desc='37' weight='bold'>Welcome, {userName}  🎉 🎉!</span>";
        welcomeLabel.SetAlignment(0.5f, 0.5f);
        fixedLayout.Put(welcomeLabel, 460, 100);

        // Button Actions
        ExplorehostelsBtn.Clicked += (sender, e) => new ExploreHostels().ShowAll();
        int currentUserId = DatabaseHelper.GetUserIdByEmail(email);
        posthostlesBtn.Clicked += (sender, e) => new PostHostles(currentUserId).ShowAll();

        myhostelsBtn.Clicked += (sender, e) => new MyHostel(currentUserId).ShowAll();

        // ✅ Fixing Profile Button Click - Fetching All User Details
        profileBtn.Clicked += (sender, e) =>
        {
            var (Name, Email, SecurityQuestion, Phone, City, Age, Gender) = DatabaseHelper.GetUserDetails(userEmail);
            new ProfileWindow(Name, Email, Phone, City, Age, Gender).ShowAll();
        };
        
        settingsBtn.Clicked += (sender, e) => new SettingsWindow().ShowAll();
        logoutBtn.Clicked += (sender, e) => 
            { 
                this.Destroy(); // ✅ Hide() ki jagah Destroy()
                new LoginWindow().ShowAll(); 
            };
        exitBtn.Clicked += (sender, e) => Application.Quit();
        submitBtn.Clicked += (sender, e) =>
{
    string name = nameEntry.Text.Trim();
    string mail = emailEntry.Text.Trim();
    string message = messageBox.Buffer.Text.Trim();

    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(message))
    {
        MessageDialog md = new MessageDialog(this, 
            DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, 
            "Please fill in all fields.");
        md.Run();
        md.Destroy();
        return;
    }

    try
    {
        int userid = DatabaseHelper.GetUserIdByEmail(mail);
        DatabaseHelper.InsertFeedback(userid, name, mail, message);

        MessageDialog success = new MessageDialog(this,
            DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
            "Thank you for your feedback!");
        success.Run();
        success.Destroy();

        // Clear fields
        nameEntry.Text = "";
        emailEntry.Text = "";
        messageBox.Buffer.Text = "";
    }
    catch (Exception ex)
    {
        MessageDialog error = new MessageDialog(this,
            DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
            "Something went wrong: " + ex.Message);
        error.Run();
        error.Destroy();
    }
};

        contactUsBtn.Clicked += (sender, e) => new Contact().ShowAll();

        Add(fixedLayout);
        ShowAll();
    }
}


class SignupWindow : Window
{
    [Obsolete]
    public SignupWindow() : base("Hostlity - Sign Up")
    {
        SetDefaultSize(1200, 500);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => Application.Quit();

        Fixed fixedLayout = new Fixed();
        try {
            Image background = new Image("signuppage.jpg");
            fixedLayout.Put(background, 0, 0);
            } catch (Exception ex) {
                        Console.WriteLine($"Failed to load background image: {ex.Message}");
                    }

        VBox vbox = new VBox(false, 15)
        {
            BorderWidth = 30
        };

        Label welcomeLabel = new Label();
        welcomeLabel.Markup = "<span font_desc='25' weight='bold'>Welcome to Hostle Wise</span>";
        welcomeLabel.SetAlignment(0.5f, 0.5f);

        Label heading = new Label();
        heading.Markup = "<span font_desc='12' weight='bold'>Enter Details to Create Your Account</span>";
        heading.SetAlignment(0.5f, 0.5f);

        Entry nameEntry = new Entry { PlaceholderText = "Full Name" };
        nameEntry.WidthRequest = 600;

        Entry emailEntry = new Entry { PlaceholderText = "Email" };
        emailEntry.WidthRequest = 600;

        Entry passwordEntry = new Entry { PlaceholderText = "Password", Visibility = false };
        passwordEntry.WidthRequest = 600;

        Entry securityQuestionEntry = new Entry { PlaceholderText = "Enter your first school friend's name" };
        securityQuestionEntry.WidthRequest = 600;  // ✅ Security Question Added

        // Show Password Checkbox
        CheckButton showPasswordCheck = new CheckButton("Show Password");
        showPasswordCheck.Toggled += (sender, e) => 
        {
            passwordEntry.Visibility = showPasswordCheck.Active;
        };

        Button signupBtn = new Button("Sign Up");
        signupBtn.WidthRequest = 320;
        signupBtn.HeightRequest = 40;

        Button loginRedirectBtn = new Button("Go to Login");
        loginRedirectBtn.WidthRequest = 320;
        loginRedirectBtn.HeightRequest = 40;

        Button exitBtn = new Button("🚪 Exit    ");
        exitBtn.WidthRequest = 120;
        exitBtn.HeightRequest = 40;
        exitBtn.Clicked += (sender, e) => Application.Quit();

        loginRedirectBtn.Clicked += (sender, e) =>
        {
            this.Hide();
            new LoginWindow();
        };

        signupBtn.Clicked += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
                string.IsNullOrWhiteSpace(emailEntry.Text) ||
                string.IsNullOrWhiteSpace(passwordEntry.Text) ||
                string.IsNullOrWhiteSpace(securityQuestionEntry.Text))  // ✅ Check Security Question
            {
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Please fill all fields!");
                md.Run();
                md.Destroy();
                return;
            }

            if (!Regex.IsMatch(emailEntry.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Invalid email format!");
                md.Run();
                md.Destroy();
                return;
            }

            if (DatabaseHelper.RegisterUser(nameEntry.Text, emailEntry.Text, passwordEntry.Text, securityQuestionEntry.Text))
            {
                MessageDialog successMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "🎉 Signup Successful!");
                successMd.Run();
                successMd.Destroy();
                this.Destroy();
                new LoginWindow();
            }
            else
            {
                MessageDialog errorMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Email already exists!");
                errorMd.Run();
                errorMd.Destroy();
            }
        };

        vbox.PackStart(welcomeLabel, false, false, 10);
        vbox.PackStart(heading, false, false, 10);
        vbox.PackStart(nameEntry, false, false, 10);
        vbox.PackStart(emailEntry, false, false, 10);
        vbox.PackStart(passwordEntry, false, false, 10);
        vbox.PackStart(securityQuestionEntry, false, false, 10);  // ✅ Security Question Field Added
        vbox.PackStart(showPasswordCheck, false, false, 10);
        vbox.PackStart(signupBtn, false, false, 10);
        vbox.PackStart(loginRedirectBtn, false, false, 10);

        fixedLayout.Put(vbox, 70, 60);
        fixedLayout.Put(exitBtn, 20, 20);

        Add(fixedLayout);
        ShowAll();
    }
}


class LoginWindow : Window
{
    [Obsolete]
    public LoginWindow() : base("Hostle Wise- Login")
    {
        SetDefaultSize(1200, 500);
        SetPosition(WindowPosition.Center);
        DeleteEvent += (o, args) => Application.Quit();

        Fixed fixedLayout = new Fixed();
        try {
            Image background = new Image("loginpage.jpg");
            fixedLayout.Put(background, 0, 0);
            } catch (Exception ex) {
                        Console.WriteLine($"Failed to load background image: {ex.Message}");
                    }

        VBox vbox = new VBox(false, 15)
        {
            BorderWidth = 30
        };

        Label heading = new Label();
        heading.Markup = "<span font_desc='25' weight='bold'>Login to Your Account</span>";
        heading.SetAlignment(0.5f, 0.5f);

        Entry emailEntry = new Entry { PlaceholderText = "Email" };
        emailEntry.WidthRequest = 600;

        Entry passwordEntry = new Entry { PlaceholderText = "Password", Visibility = false };
        passwordEntry.WidthRequest = 600;

        CheckButton showPasswordCheck = new CheckButton("Show Password");
        showPasswordCheck.Toggled += (sender, e) =>
        {
            passwordEntry.Visibility = showPasswordCheck.Active;
        };

        Button loginBtn = new Button("Login");
        loginBtn.WidthRequest = 320;
        loginBtn.HeightRequest = 40;

        Button signupRedirectBtn = new Button("Go to Signup");
        signupRedirectBtn.WidthRequest = 320;
        signupRedirectBtn.HeightRequest = 40;

        Button forgotpassword = new Button("🔑  Forget Password");
        forgotpassword.WidthRequest = 320;
        forgotpassword.HeightRequest = 40;
        
        
        Button exitBtn = new Button("🚪 Exit");
        exitBtn.WidthRequest = 120;
        exitBtn.HeightRequest = 40;
        exitBtn.Clicked += (sender, e) => Application.Quit();
        #pragma warning disable CS8622 
        forgotpassword.Clicked += OnForgetPasswordClicked;
        #pragma warning restore CS8622

        signupRedirectBtn.Clicked += (sender, e) =>
        {
            this.Hide();
            new SignupWindow().ShowAll();
        };

        loginBtn.Clicked += (sender, e) =>
{
    if (string.IsNullOrWhiteSpace(emailEntry.Text) ||
        string.IsNullOrWhiteSpace(passwordEntry.Text))  
    {
        MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️  Please enter email and password!");
        md.Run();
        md.Destroy();
        return;
    }

    if (!Regex.IsMatch(emailEntry.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
    {
        MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️  Invalid email format!");
        md.Run();
        md.Destroy();
        return;
    }

    if (DatabaseHelper.AuthenticateUser(emailEntry.Text, passwordEntry.Text))
    {
        // ✅ Fetch User Details with Correct Tuple Size
        var (Name, Email, SecurityQuestion, Phone, City, Age, Gender) = DatabaseHelper.GetUserDetails(emailEntry.Text);

        MessageDialog successMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, $"🎉 Login Successful! Welcome {Name}!");
        successMd.Run();
        successMd.Destroy();

        this.Hide();
        new DashboardWindow(Name, Email).ShowAll();  // ✅ No issues now
    }
    else
    {
        MessageDialog errorMd = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️  Invalid credentials!");
        errorMd.Run();
        errorMd.Destroy();
    }

    
};
        vbox.PackStart(heading, false, false, 10);
        vbox.PackStart(emailEntry, false, false, 10);
        vbox.PackStart(passwordEntry, false, false, 10);
        vbox.PackStart(showPasswordCheck, false, false, 10);
        vbox.PackStart(loginBtn, false, false, 10);
        vbox.PackStart(signupRedirectBtn, false, false, 10);
        vbox.PackStart(forgotpassword, false, false, 10);

        fixedLayout.Put(vbox, 70, 150);
        fixedLayout.Put(exitBtn, 20, 20);

        Add(fixedLayout);
        ShowAll();
    }

    // Function to check and update the forgot password Query:
    private void OnForgetPasswordClicked(object sender, EventArgs e)
    {
        Dialog dialog = new Dialog("Forget Password", this, DialogFlags.Modal);
        dialog.SetDefaultSize(500, 400);
        
        var provider = new CssProvider();
        provider.LoadFromData("dialog { background-color: black; } label { color: white; }");
        dialog.StyleContext.AddProvider(provider, StyleProviderPriority.User);
        
        var contentArea = dialog.ContentArea;
        Entry emailEntry = new Entry { PlaceholderText = "Enter your email" };
        Entry securityQuestionEntry = new Entry { PlaceholderText = "Enter your security answer" };
        Entry newPasswordEntry = new Entry { PlaceholderText = "Enter new password", Visibility = false };
        CheckButton showPasswordCheck = new CheckButton("Show Password");
        showPasswordCheck.Toggled += (s, ev) => newPasswordEntry.Visibility = showPasswordCheck.Active;
        
        contentArea.PackStart(new Label("Enter details to reset your password:"), false, false, 10);
        contentArea.PackStart(emailEntry, false, false, 5);
        contentArea.PackStart(securityQuestionEntry, false, false, 5);
        contentArea.PackStart(newPasswordEntry, false, false, 5);
        contentArea.PackStart(showPasswordCheck, false, false, 5);
        
        dialog.AddButton("Cancel", ResponseType.Cancel);
        dialog.AddButton("Reset Password", ResponseType.Ok);
        dialog.ShowAll();
        
        if (dialog.Run() == (int)ResponseType.Ok)
        {
            if (DatabaseHelper.ResetPassword(emailEntry.Text, securityQuestionEntry.Text, newPasswordEntry.Text))
            {
                MessageDialog success = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "🎉 Password Reset Successful!");
                success.Run();
                success.Destroy();
            }
            else
            {
                MessageDialog error = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "⚠️ Invalid details!");
                error.Run();
                error.Destroy();
            }
        }
        dialog.Destroy();
    }
}

class Program
{
    [Obsolete]
    static void Main()
    {   DatabaseHelper.InitializeDatabase();
        Application.Init();
        new WelcomeWindow();
        Application.Run();
    }
}
