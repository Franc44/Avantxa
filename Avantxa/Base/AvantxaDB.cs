using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
namespace Avantxa.Base
{
    public class AvantxaDB
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public AvantxaDB()
        {
            InitializeAsync().SafeFireAndForget(false);
            Initialize().SafeFireAndForget(false);
            InitializeVer().SafeFireAndForget(false);
            InitializeCalendario().SafeFireAndForget(false);
        }
        public async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(TablaUsuario).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(TablaUsuario)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }
        public async Task Initialize()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(TablaMensajes).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(TablaMensajes)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public async Task InitializeVer()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(TablaVerMensajes).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(TablaVerMensajes)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public async Task InitializeCalendario()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(TablaCalendario).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(TablaCalendario)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public Task<int> SaveItemAsync(TablaUsuario item)
        {
            return Database.InsertAsync(item);
        }

        public Task<TablaUsuario> GetItemsAsync()
        {
            //return Database.Table<TablaUsuario>().ToListAsync();
            return Database.Table<TablaUsuario>().FirstOrDefaultAsync();
        }

        public Task<int> DeleteItemAsync()
        {
            return Database.DeleteAllAsync<TablaUsuario>();
        }
        public Task<int> SaveItemAsyncMen(TablaMensajes item)
        {
            return Database.InsertAsync(item);
        }

        public Task<List<TablaMensajes>> GetItemsAsyncMen()
        {
            return Database.Table<TablaMensajes>().ToListAsync();
        }

        public Task<int> DeleteItemAsyncMen()
        {
            return Database.DeleteAllAsync<TablaMensajes>();
        }
        public Task<int> SaveItemAsyncVer(TablaVerMensajes item)
        {
            return Database.InsertAsync(item);
        }

        public Task<List<TablaVerMensajes>> GetItemsAsyncVer()
        {
            return Database.Table<TablaVerMensajes>().ToListAsync();
        }

        public Task<int> DeleteItemAsyncVer()
        {
            return Database.DeleteAllAsync<TablaVerMensajes>();
        }

        public Task<int> SaveItemAsyncCal(TablaCalendario item)
        {
            return Database.InsertAsync(item);
        }

        public Task<List<TablaCalendario>> GetItemsAsyncCal()
        {
            return Database.Table<TablaCalendario>().ToListAsync();
        }

        public Task<int> DeleteItemAsyncCal()
        {
            return Database.DeleteAllAsync<TablaCalendario>();
        }
    }
}
