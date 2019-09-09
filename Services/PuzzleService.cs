using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshBoard.Data;
using Microsoft.EntityFrameworkCore;
using DbContext = FreshBoard.Data.DbContext;

namespace FreshBoard.Services
{
    public interface IPuzzleService
    {
        Task<int> CreateProblemAsync(Problem problem);
        Task RemoveProblemAsync(int problemId);
        Task UpdateProblemAsync(Problem problem);
        IQueryable<Problem> QueryProblem();
        IAsyncEnumerable<Problem> GetProblemsByLevelAsync(int level);
        Task<Problem?> GetProblemByIdAsync(int id);
        Task RecordSubmission(int problemId, string userId, string? answer, int result);
    }
    public class PuzzleService : IPuzzleService
    {
        private readonly DbContext _dbContext;

        public PuzzleService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateProblemAsync(Problem problem)
        {
            await _dbContext.Problem.AddAsync(problem);
            await _dbContext.SaveChangesAsync();
            return problem.Id;
        }

        public async Task RemoveProblemAsync(int problemId)
        {
            var problem = await _dbContext.Problem.FirstOrDefaultAsync(i => i.Id == problemId);
            if (problem == null) return;
            _dbContext.Problem.Remove(problem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateProblemAsync(Problem problem)
        {
            if (!await _dbContext.Problem.AnyAsync(i => i.Id == problem.Id)) return;
            _dbContext.Problem.Update(problem);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Problem> QueryProblem() => _dbContext.Problem;

        public IAsyncEnumerable<Problem> GetProblemsByLevelAsync(int level) => _dbContext.Problem.Where(i => i.Level == level).AsAsyncEnumerable();

        public Task<Problem?> GetProblemByIdAsync(int id) => _dbContext.Problem.FirstOrDefaultAsync(i => i.Id == id);

        public Task RecordSubmission(int problemId, string userId, string? answer, int result)
        {
            _dbContext.PuzzleRecord.Add(new PuzzleRecord
            {
                ProblemId = problemId,
                Content = answer,
                Result = result,
                Time = DateTime.Now,
                UserId = userId
            });
            return _dbContext.SaveChangesAsync();
        }
    }
}
