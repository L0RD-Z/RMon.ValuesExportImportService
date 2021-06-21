using RMon.Commissioning.Core;

namespace RMon.ValuesExportImportService.Processing.Permission
{
    class PermissionConstant
    {
        /// <summary>
        /// Тип сущности
        /// </summary>
        public EntityTypes EntityType { get; set; }
        /// <summary>
        /// Тип операции над сущностью
        /// </summary>
        public CrudOperations CrudOperation { get; set; }
        /// <summary>
        /// Строковая константа 
        /// </summary>
        public string Constant { get; set; }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="crudOperation">Тип операции над сущностью</param>
        /// <param name="constant">Строковая константа </param>
        public PermissionConstant(EntityTypes entityType, CrudOperations crudOperation, string constant)
        {
            EntityType = entityType;
            CrudOperation = crudOperation;
            Constant = constant;
        }
    }
}
