using Microsoft.AspNetCore.Mvc;
using TaskRtUpdater.src.Application.DTOs;
using TaskRtUpdater.src.Application.UseCases;
using TaskRtUpdater.src.Infrastructure.Repositories;

namespace TaskRtUpdater.src.Presentation.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _repository;
        private readonly CreateTaskUseCase _createTaskUseCase;
        private readonly UpdateTaskUseCase _updateTaskUseCase;
        private readonly DeleteTaskUseCase _deleteTaskUseCase;

        public TasksController(
            ITaskRepository repository,
            CreateTaskUseCase createTaskUseCase,
            UpdateTaskUseCase updateTaskUseCase,
            DeleteTaskUseCase deleteTaskUseCase)
        {
            _repository = repository;
            _createTaskUseCase = createTaskUseCase;
            _updateTaskUseCase = updateTaskUseCase;
            _deleteTaskUseCase = deleteTaskUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var task = await _createTaskUseCase.ExecuteAsync(dto);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Domain.Exceptions.CircularDependencyException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Domain.Exceptions.TaskNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _repository.GetAllAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _repository.GetByIdAsync(id);
                return Ok(task);
            }
            catch (Domain.Exceptions.TaskNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            try
            {
                var task = await _updateTaskUseCase.ExecuteAsync(id, dto);
                return Ok(task);
            }
            catch (Domain.Exceptions.TaskNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Domain.Exceptions.InvalidTaskStatusException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _deleteTaskUseCase.ExecuteAsync(id);
                return NoContent();
            }
            catch (Domain.Exceptions.TaskNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("crit")]
        public async Task<IActionResult> GetCriticalPath()
        {
            try
            {
                var criticalPath = await _repository.GetCriticalPath();
                var ids = criticalPath.Select(t => t.Id).ToList();
                return Ok(ids);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
