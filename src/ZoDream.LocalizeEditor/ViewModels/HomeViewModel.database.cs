using System;
using System.Windows.Input;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Readers.Database;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {
        private Action<string, IReader>? DatabaseDialogCallbackFn;
        private bool databaseDialogVisible;

        public bool DatabaseDialogVisible {
            get => databaseDialogVisible;
            set {
                Set(ref databaseDialogVisible, value);
            }
        }

        private string[] databaseTypeItems = new[] {"MySQL", /*"MsSQL", "Access"*/}; 

        public string[] DatabaseTypeItems {
            get => databaseTypeItems;
            set => Set(ref databaseTypeItems, value);
        }

        private string[] databaseSchemaItems = Array.Empty<string>();

        public string[] DatabaseSchemaItems {
            get => databaseSchemaItems;
            set => Set(ref databaseSchemaItems, value);
        }

        private string[] databaseTableItems = Array.Empty<string>();

        public string[] DatabaseTableItems {
            get => databaseTableItems;
            set => Set(ref databaseTableItems, value);
        }

        private string[] databaseFieldItems = Array.Empty<string>();

        public string[] DatabaseFieldItems {
            get => databaseFieldItems;
            set => Set(ref databaseFieldItems, value);
        }


        private string databaseType = string.Empty;

        public string DatabaseType {
            get => databaseType;
            set => Set(ref databaseType, value);
        }

        private string databaseHost = "127.0.0.1";

        public string DatabaseHost {
            get => databaseHost;
            set => Set(ref databaseHost, value);
        }

        private string databaseUsername = "root";

        public string DatabaseUsername {
            get => databaseUsername;
            set => Set(ref databaseUsername, value);
        }

        private string databasePassword = string.Empty;

        public string DatabasePassword {
            get => databasePassword;
            set => Set(ref databasePassword, value);
        }

        private string databaseSchema = string.Empty;

        public string DatabaseSchema {
            get => databaseSchema;
            set => Set(ref databaseSchema, value);
        }

        private string databaseTable = string.Empty;

        public string DatabaseTable {
            get => databaseTable;
            set => Set(ref databaseTable, value);
        }

        private string databaseID = "id";

        public string DatabaseID {
            get => databaseID;
            set => Set(ref databaseID, value);
        }

        private string databaseSource = "source";

        public string DatabaseSource {
            get => databaseSource;
            set => Set(ref databaseSource, value);
        }

        private string databaseTarget = "target";

        public string DatabaseTarget {
            get => databaseTarget;
            set => Set(ref databaseTarget, value);
        }

        public ICommand DatabaseDialogConfirmCommand { get; private set; }


        public void DatabaseDialogOpen(Action<string, IReader>? callback = null)
        {
            DatabaseDialogVisible = true;
            DatabaseDialogCallbackFn = callback;
        }


        private void TapDatabaseDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(DatabaseSchema) || string.IsNullOrWhiteSpace(DatabaseTable))
            {
                return;
            }
            DialogVisible = false;
            if (DialogCallbackFn is null)
            {
                return;
            }
            var reader = DatabaseInstance;
            if (reader is null)
            {
                return;
            }
            DatabaseDialogCallbackFn?.Invoke(reader.ConnectStringBuilder(DatabaseHost, DatabaseUsername, DatabasePassword,
                    DatabaseSchema, DatabaseTable, DatabaseID, DatabaseSource, DatabaseTarget), 
                    reader);
        }

        private IDatabaseReader? DatabaseInstance => DatabaseType switch
        {
            "MySQL" => new MySQLReader(),
            _ => null
        };
    }
}
