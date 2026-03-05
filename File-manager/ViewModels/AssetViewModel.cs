using File_manager.Interfaces;
using File_manager.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;


namespace File_manager.ViewModels
{
    public class AssetViewModel
    {
        private readonly IFileWatcher _watcher;
        private readonly IRepository<IAsset> _repository;
        private readonly IStatusEvaluator _evaluator;


        public ObservableCollection<IAsset> Assets { get; } = new ();
        public ICommand? ApproveCommand { get;  }
        public ICommand? RejectCommand { get;  }
        public ICommand? MarkDoneCommand { get;  }



        public AssetViewModel(IFileWatcher watcher, IRepository<IAsset> repository, IStatusEvaluator evaluator)
        {
            _watcher = watcher;
            _repository = repository;        
            _evaluator = evaluator;

            _watcher.OnFileSystemChanged += HandleFileChange;
        }  // для ініціалізації полів

        private void HandleFileChange(FileSystemEventArgs e)
        {
            
        }
    }
}
