using Dapper;
using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.Interfaces.IReadModelRepository;
using NazarMahal.Core.ReadModels;
using System.Data;
using System.Text;

namespace NazarMahal.Infrastructure.ReadModelRepository
{
    public class GlassesReadModelRepository(IDbConnection dbConnection) : IGlassesReadModelRepository
    {
        private readonly IDbConnection _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

        public async Task<IReadOnlyList<GlassesAttachmentReadModel>> GetGlassesAttachments(int glassesId)
        {
            const string sqlQuery = @"
                SELECT 
                    [Id],
                    [GlassesId],
                    [FileName],
                    [FileType],
                    [StoragePath]
                FROM [dbo].[GlassesAttachment]
                WHERE [GlassesId] = @GlassesId";

            var result = await _dbConnection.QueryAsync<GlassesAttachmentReadModel>(
                sqlQuery,
                new { GlassesId = glassesId });

            return result.ToList().AsReadOnly();
        }

        public async Task<GlassesReadModel> GetGlassesById(int glassesId)
        {
            const string sqlQuery = @"
                SELECT 
                    g.Id AS GlassesId,
                    g.Name AS GlassesName,
                    g.Description,
                    g.Price,
                    g.Brand,
                    g.Model,
                    g.FrameType,
                    g.LensType,
                    g.Color,
                    g.CategoryId,
                    gc.Name AS CategoryName,
                    g.SubCategoryId,
                    gsc.Name AS SubCategoryName,
                    g.IsActive,
                    g.AvailableQuantity,
                    ga.Id AS AttachmentId,
                    ga.FileName,
                    ga.FileType,
                    ga.StoragePath
                FROM dbo.Glasses g
                INNER JOIN dbo.GlassesCategory gc ON g.CategoryId = gc.Id
                INNER JOIN dbo.GlassesSubcategory gsc ON g.SubCategoryId = gsc.Id
                LEFT JOIN dbo.GlassesAttachment ga ON g.Id = ga.GlassesId
                WHERE g.Id = @GlassesId";

            var result = await _dbConnection.QueryAsync<GlassesReadModel>(
                sqlQuery,
                new { GlassesId = glassesId });

            return result.FirstOrDefault();
        }

        public async Task<IReadOnlyList<GlassesCategoryReadModel>> GetGlassesCategories(bool showActiveOnly, bool showHavingSubcategories)
        {
            var query = @$"SELECT gc.Id, 
                                  gc.Name, 
                                  gc.IsActive
                           FROM [dbo].[GlassesCategory] gc
                           WHERE 1 = 1
                           {(showActiveOnly ? " AND gc.IsActive = 1" : "")}
                           {(showHavingSubcategories ? " AND EXISTS (SELECT 1 FROM GlassesSubcategory gsc WHERE gsc.CategoryId = gc.Id AND gsc.IsActive = 1)" : "")}";

            var result = await _dbConnection.QueryAsync<GlassesCategoryReadModel>(query);

            return result.ToList();
        }

        public async Task<GlassesCategoryReadModel> GetGlassesCategoryById(int categoryId)
        {
            const string query = @"
                SELECT vw.Id, 
                       vw.Name, 
                       vw.IsActive
                FROM [dbo].[GlassesCategory] vw
                WHERE Id = @CategoryId";

            try
            {
                var result = await _dbConnection.QueryAsync<GlassesCategoryReadModel>(query, new { CategoryId = categoryId });
                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<GlassesSubcategoryReadModel>> GetGlassesSubcategories(bool showActiveOnly, int categoryId)
        {
            var queryBuilder = new StringBuilder(@"
                SELECT Id, 
                       Name, 
                       CategoryId, 
                       IsActive
                FROM [dbo].[GlassesSubcategory]
                WHERE 1 = 1");

            var parameters = new DynamicParameters();

            if (showActiveOnly)
            {
                queryBuilder.Append(" AND IsActive = 1");
            }

            if (categoryId > 0)
            {
                queryBuilder.Append(" AND CategoryId = @CategoryId");
                parameters.Add("CategoryId", categoryId);
            }

            var result = await _dbConnection.QueryAsync<GlassesSubcategoryReadModel>(queryBuilder.ToString(), parameters);
            return result.ToList();
        }

        public async Task<IReadOnlyList<GlassesSubcategoryReadModel>> GeAllGlassesSubcategoriesByCategoryId(int categoryId)
        {
            const string query = @"
                SELECT Id, 
                       Name, 
                       CategoryId, 
                       IsActive
                FROM [dbo].[GlassesSubcategory]
                WHERE CategoryId = @CategoryId";

            var result = await _dbConnection.QueryAsync<GlassesSubcategoryReadModel>(query, new { CategoryId = categoryId });
            return result.ToList();
        }

        public async Task<GlassesSubcategoryReadModel> GetGlassesSubcategoryById(int subcategoryId, int categoryId = 0)
        {
            var query = @"
                SELECT gsc.Id, 
                       gsc.Name, 
                       gsc.CategoryId,
                       gsc.IsActive
                FROM [dbo].[GlassesSubcategory] gsc
                WHERE gsc.Id = @SubcategoryId";

            if (categoryId > 0)
            {
                query += " AND gsc.CategoryId = @CategoryId";
            }

            try
            {
                var result = await _dbConnection.QueryAsync<GlassesSubcategoryReadModel>(query,
                    new { SubcategoryId = subcategoryId, CategoryId = categoryId });

                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<GlassesReadModel>> GetGlassesAsync(SearchGlassessDto searchGlasses)
        {
            var sql = new StringBuilder(@"
        SELECT 
            g.Id AS GlassesId, 
            g.Name AS GlassesName, 
            g.Description, 
            g.Price, 
            g.Brand, 
            g.Model,
            g.FrameType, 
            g.LensType, 
            g.Color, 
            g.CategoryId, 
            gc.Name AS CategoryName,
            g.SubCategoryId, 
            gsc.Name AS SubCategoryName, 
            g.IsActive, 
            g.AvailableQuantity,
            ga.Id AS AttachmentId, 
            ga.FileName, 
            ga.FileType, 
            ga.StoragePath
        FROM dbo.Glasses g
        INNER JOIN dbo.GlassesCategory gc ON g.CategoryId = gc.Id
        LEFT JOIN dbo.GlassesSubcategory gsc ON g.SubCategoryId = gsc.Id AND gsc.IsActive = 1
        LEFT JOIN dbo.GlassesAttachment ga ON g.Id = ga.GlassesId
        WHERE 1 = 1");

            var parameters = new DynamicParameters();

            void AddFilter(string column, string paramName, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    sql.Append($" AND {column} LIKE @{paramName}");
                    parameters.Add(paramName, $"%{value}%");
                }
            }

            // Add filters
            AddFilter("g.Name", "GlassesName", searchGlasses.GlassesName);
            AddFilter("g.Brand", "Brand", searchGlasses.Brand);
            AddFilter("g.Description", "Description", searchGlasses.Description);
            AddFilter("g.Color", "Color", searchGlasses.Color);
            AddFilter("g.FrameType", "Style", searchGlasses.Style); // Map Style to FrameType
            AddFilter("gc.Name", "CategoryName", searchGlasses.CategoryName);
            AddFilter("gsc.Name", "SubCategoryName", searchGlasses.SubCategoryName);

            // Add exact match filters
            if (searchGlasses.CategoryId.HasValue && searchGlasses.CategoryId > 0)
            {
                sql.Append(" AND g.CategoryId = @CategoryId");
                parameters.Add("CategoryId", searchGlasses.CategoryId);
            }

            if (searchGlasses.SubCategoryId.HasValue && searchGlasses.SubCategoryId > 0)
            {
                sql.Append(" AND g.SubCategoryId = @SubCategoryId");
                parameters.Add("SubCategoryId", searchGlasses.SubCategoryId);
            }

            if (searchGlasses.IsActive.HasValue)
            {
                sql.Append(" AND g.IsActive = @IsActive");
                parameters.Add("IsActive", searchGlasses.IsActive);
            }

            // Add sorting
            sql.Append(" ORDER BY g.Id DESC");

            // Add pagination
            if (searchGlasses.PageSize.HasValue && searchGlasses.PageSize > 0)
            {
                int offset = (searchGlasses.PageNum - 1) * searchGlasses.PageSize.Value;
                sql.Append($" OFFSET {offset} ROWS FETCH NEXT {searchGlasses.PageSize} ROWS ONLY");
            }

            var glassesDictionary = new Dictionary<int, GlassesReadModel>();

            var result = await _dbConnection.QueryAsync<GlassesReadModel, GlassesAttachmentReadModel, GlassesReadModel>(
                sql.ToString(),
                (glasses, attachment) =>
                {
                    if (!glassesDictionary.TryGetValue(glasses.GlassesId, out var existing))
                    {
                        existing = glasses;
                        existing.AttachmentReadModels = new List<GlassesAttachmentReadModel>();
                        glassesDictionary.Add(existing.GlassesId, existing);
                    }

                    if (attachment != null && attachment.AttachmentId != 0)
                    {
                        existing.AttachmentReadModels.Add(attachment);
                    }

                    return existing;
                },
                parameters,
                splitOn: "AttachmentId"
            );

            return glassesDictionary.Values.ToList().AsReadOnly();
        }

        private string GetSearchString(string input) => !string.IsNullOrEmpty(input) ? $"%{input}%" : null;
    }
}
