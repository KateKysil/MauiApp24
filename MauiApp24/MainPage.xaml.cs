//using Android.Icu.Text;
using Parsers;
using Saver;
using System.Text;
using LogLibrary;
namespace MauiApp24
{
    public partial class MainPage : ContentPage
    {

        string _filePath;
        ScheduleService _scheduleService = new ScheduleService(new DOMParsingStrategy());
        List<Subject> results = new List<Subject>();
        List<Subject> resultsEdited = new List<Subject>();
        List<Subject> resultsHTML = new List<Subject>();
        Logger logger = Logger.Instance;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoadFileClicked(object sender, EventArgs e)
        {
            string? filePath = await FileLoader.SelectFileAsync();
            if (!string.IsNullOrEmpty(filePath))
            {
                _filePath = filePath;
                selectedLbl.Text = $"Selected file is {_filePath}";
                try
                {
                    results = _scheduleService.LoadSchedule(_filePath);
                    ParsedTableGrid.Children.Clear();
                    var scheduleGrid = CreateScheduleGrid(results);
                    ParsedTableGrid.Children.Add(scheduleGrid);
                    DayPicker.IsEnabled = true;
                    GroupPicker.IsEnabled = true;
                    SubjectPicker.IsEnabled = true;
                    TeacherPicker.IsEnabled = true;
                    TimePicker.IsEnabled = true;
                    DayPicker.ItemsSource = new List<string>();
                    GroupPicker.ItemsSource = new List<string>();
                    SubjectPicker.ItemsSource = new List<string>();
                    TeacherPicker.ItemsSource = new List<string>();
                    TimePicker.ItemsSource = new List<string>();
                    resultsEdited = results.ToList();
                    resultsHTML = results.ToList();
                    List<string> days = new List<string>();
                    List<string> groups = new List<string>();
                    List<string> subs = new List<string>();
                    List<string> teachers = new List<string>();
                    List<string> times = new List<string>();
                    foreach (var res in resultsEdited)
                    {
                        days.Add(res.Day);
                        groups.Add(res.Group);
                        subs.Add(res.Name);
                        foreach (var t in res.Teachers)
                        {
                            teachers.Add(t.Name);
                        }
                        times.Add(res.Time);
                    }
                    days.Add("all");
                    groups.Add("all");
                    subs.Add("all");
                    teachers.Add("all");
                    times.Add("all");
                    HashSet<string> uniqueDays = new HashSet<string>(days);
                    DayPicker.ItemsSource = uniqueDays.ToList();
                    HashSet<string> uniqueGroups = new HashSet<string>(groups);
                    GroupPicker.ItemsSource = uniqueGroups.ToList();
                    HashSet<string> uniqueSub = new HashSet<string>(subs);
                    SubjectPicker.ItemsSource = uniqueSub.ToList();
                    HashSet<string> uniqueTeachers = new HashSet<string>(teachers);
                    TeacherPicker.ItemsSource = uniqueTeachers.ToList();
                    HashSet<string> uniqueTime = new HashSet<string>(times);
                    TimePicker.ItemsSource = uniqueTime.ToList();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fail", ex.Message, "Ok");
                }
                
            }
            else
            {
                DayPicker.IsEnabled = false;
                GroupPicker.IsEnabled = false;
                SubjectPicker.IsEnabled = false;
                TeacherPicker.IsEnabled = false;
                TimePicker.IsEnabled = false;
            }
        }
        public static Grid CreateScheduleGrid(List<Subject> subjects)
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Auto }, 
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };
            AddGridHeader(grid, "Day", 0);
            AddGridHeader(grid, "Group", 1);
            AddGridHeader(grid, "Subject", 2);
            AddGridHeader(grid, "Teachers", 3);
            AddGridHeader(grid, "Cabinets", 4);
            AddGridHeader(grid, "Time", 5);

            int row = 1;
            foreach (var subject in subjects)
            {
                var teachersText = string.Join("; ", subject.Teachers.Select(t => $"{t.Name}, {t.Position}"));
                var cabinetsText = string.Join("; ", subject.Teachers.Select(t => t.Room));

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                AddGridCell(grid, subject.Day, row, 0);
                AddGridCell(grid, subject.Group, row, 1);
                AddGridCell(grid, subject.Name, row, 2);
                AddGridCell(grid, teachersText, row, 3);
                AddGridCell(grid, cabinetsText, row, 4);
                AddGridCell(grid, subject.Time, row, 5);

                row++;
            }
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int col = 0; col < grid.ColumnDefinitions.Count; col++)
            {
                AddGridCell(grid, string.Empty, row, col);
            }

            return grid;
        }

        private static void AddGridHeader(Grid grid, string header, int column)
        {
            var label = new Label
            {
                Text = header,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(5, 5) 
            };

            grid.Children.Add(label);
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, column);
        }

        private static void AddGridCell(Grid grid, string text, int row, int column)
        {
            var label = new Label
            {
                Text = text,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(5, 5) 
            };

            grid.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
        }

        public void SaveSubjectsAsHtmlTable(string filePath, List<Subject> subjects)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Schedule</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Schedule</h1>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Day</th><th>Group</th><th>Subject</th><th>Teachers</th><th>Cabinets</th><th>Time</th></tr>");
            foreach (var subject in subjects)
            {
                var teachers = string.Join("; ", subject.Teachers.Select(t => $"{t.Name} ({t.Position})"));
                var cabinets = string.Join("; ", subject.Teachers.Select(t => t.Room));
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{subject.Day}</td>");
                sb.AppendLine($"<td>{subject.Group}</td>");
                sb.AppendLine($"<td>{subject.Name}</td>");
                sb.AppendLine($"<td>{teachers}</td>");
                sb.AppendLine($"<td>{cabinets}</td>");
                sb.AppendLine($"<td>{subject.Time}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            File.WriteAllText(filePath, sb.ToString());
            logger.Log("Збереження", $"Saved in {filePath}");
        }


        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Виконала Кисіль Катерина К-25", "OK");
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Exit", "Are you sure you want to exit?", "Yes", "No"))
            {
                Application.Current?.Quit();
            }
        }

        private async void OnParserTypeChanged(object sender, CheckedChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                await DisplayAlert("Error", $"Select a file before searching.", "OK");
                return;
            }
            if (rbDOM.IsChecked == true)
                _scheduleService = new ScheduleService(new DOMParsingStrategy());
            else if(rbLINQ.IsChecked == true)
                _scheduleService = new ScheduleService(new LINQParsingStrategy());
            else if(rbSAX.IsChecked == true)
                _scheduleService = new ScheduleService(new SAXParsingStrategy());
            _scheduleService.LoadSchedule(_filePath);
            DayPicker.SelectedItem = null;
            GroupPicker.SelectedItem = null;
            SubjectPicker.SelectedItem = null;
            TeacherPicker.SelectedItem = null;
            TimePicker.SelectedItem = null;
            ParsedTableGrid.Children.Clear();
            var scheduleGrid = CreateScheduleGrid(results);
            ParsedTableGrid.Children.Add(scheduleGrid);
        }
        private void OnDaySelected(object sender, EventArgs e)
        {
            OnSelected();
        }
        private void OnSelected()
        {
            var selectedDay = DayPicker.SelectedItem?.ToString();
            var selectedGroup = GroupPicker.SelectedItem?.ToString();
            var selectedSubject = SubjectPicker.SelectedItem?.ToString();
            var selectedTeacher = TeacherPicker.SelectedItem?.ToString();
            var selectedTime = TimePicker.SelectedItem?.ToString();
            string select = "Параметри: ";

            if (!string.IsNullOrEmpty(selectedDay) && selectedDay!="all")
            {
                resultsEdited.RemoveAll(res => res.Day != selectedDay);
                select += ("Day = \""+selectedDay+"\",");
            }
            if (!string.IsNullOrEmpty(selectedGroup) && selectedGroup!="all")
            {
                resultsEdited.RemoveAll(res => res.Group != selectedGroup);
                select += ("Group = \"" + selectedGroup + "\",");
            }
            if (!string.IsNullOrEmpty(selectedSubject) && selectedSubject != "all")
            {
                resultsEdited.RemoveAll(res => res.Name != selectedSubject);
                select += ("Subject = \"" + selectedSubject + "\",");
            }
            if (!string.IsNullOrEmpty(selectedTeacher) && selectedTeacher !="all")
            {
                resultsEdited.RemoveAll(subject => !subject.Teachers.Any(teacher => teacher.Name == selectedTeacher));
                select += ("Teacher = \"" + selectedTeacher + "\",");
            }
            if (!string.IsNullOrEmpty(selectedTime) && selectedTime != "all")
            {
                resultsEdited.RemoveAll(res => res.Time != selectedTime);
                select += ("Time = \"" + selectedTime + "\",");
            }
            ParsedTableGrid.Children.Clear();
            var scheduleGrid = CreateScheduleGrid(resultsEdited);
            ParsedTableGrid.Children.Add(scheduleGrid);
            resultsHTML = resultsEdited.ToList();
            resultsEdited = results.ToList();
            logger.Log("Фільтрація", select);
        }

        private void OnGroupSelected(object sender, EventArgs e)
        {
            OnSelected();
        }

        private void OnSubjectSelected(object sender, EventArgs e)
        {
            OnSelected();
        }

        private void OnTeacherSelected(object sender, EventArgs e)
        {
            OnSelected();
        }

        private void OnTimeSelected(object sender, EventArgs e)
        {
            OnSelected();
        }
        private void OnClearClicked(object sender, EventArgs e)
        {
            DayPicker.SelectedItem = null;
            GroupPicker.SelectedItem = null;
            SubjectPicker.SelectedItem = null;
            TeacherPicker.SelectedItem = null;
            TimePicker.SelectedItem = null;
            OnSelected();
        }

        private async void OnHTMLButtonClicked(object sender, EventArgs e)
        {
            if(selectedLbl.Text != "")
            {
                string selectedFormat = await DisplayActionSheet("Select Format", "Cancel", null, "HTML", "XML");

                if (selectedFormat == "Cancel")
                    return;
                var googleDriveSaver = new GoogleDriveSaver();
                await googleDriveSaver.SaveToGoogleDriveAsync(resultsHTML, selectedFormat);
                await DisplayAlert("Success", $"File saved to Google Drive as {selectedFormat}", "OK");
            }
            else
            {
                await DisplayAlert("Fail", "File is not chosen", "OK");
            }
        }

    }

}
