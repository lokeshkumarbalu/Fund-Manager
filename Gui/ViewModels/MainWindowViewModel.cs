namespace FundManager.Gui.ViewModels
{
    using FundManager.Domain;
    using FundManager.Gui.Constants;
    using FundManager.Logging;
    using FundManager.Store;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Linq;

    internal class MainWindowViewModel
    {
        private readonly IPersonStore personStore;

        public Person SelectedPerson { get; set; }

        public IList<Person> People { get; set; }

        public MainWindowViewModel()
        {
            this.People = new ObservableCollection<Person>();
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dataTransferPath = System.IO.Path.Combine(documents, "FundManager");
            var dataTransferDirectoryInfo = new DirectoryInfo(dataTransferPath);
            var log = new CompositeLog(LoggerNames.DbLoggerName);
            this.personStore = new PersonStore(dataTransferDirectoryInfo, log);

            var peopleResult = this.personStore.GetAll();
            if (peopleResult.IsOk)
            {
                peopleResult.ResultValue
                    .ToList()
                    .ForEach(x => this.People.Add(x));

                this.SelectedPerson = peopleResult.ResultValue.Head;
            }
        }
    }
}
