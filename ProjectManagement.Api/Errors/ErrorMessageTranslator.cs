namespace ProjectManagement.Api.Errors;

public static class ErrorMessageTranslator
{
    public static string Translate(string messageKey) =>
        messageKey switch
        {
            // --- PROYECTOS (DomainErrors.Project) ---
            "project.name.required.message" => "El nombre del proyecto es obligatorio.",
            "project.name.too_long.message" => "El nombre del proyecto excede la longitud máxima permitida.",
            "project.owner.required.message" => "El ID del propietario del proyecto es obligatorio.",
            "project.id.required.message" => "El ID del proyecto es obligatorio.",
            "project.has_in_progress_tasks.message" => "No se puede archivar un proyecto que tiene tareas en curso (InProgress).",
            "project.is_archived.message" => "No se pueden añadir tareas a un proyecto que ya está archivado.",

            // --- TAREAS (DomainErrors.TaskItem) ---
            "task.title.required.message" => "El título de la tarea es obligatorio.",
            "task.title.too_long.message" => "El título de la tarea excede la longitud máxima permitida.",
            "task.priority.invalid.message" => "La prioridad seleccionada no es válida.",
            "task.due_date_in_past.message" => "La fecha de vencimiento de la tarea no puede estar en el pasado.",
            "task.id.required.message" => "El ID de la tarea es obligatorio.",
            "task.status.invalid.message" => "El estado de la tarea no es válido.",

            // --- GENÉRICOS DE APPLICATION (ApplicationErrors) ---
            "entity.not_found_message" => "El recurso solicitado no ha sido encontrado en el sistema.",
            "validation.one_or_more_errors_occurred" => "Se han producido uno o más errores de validación en la solicitud.",
            "validation.failed" => "La validación de la solicitud ha fallado.",

            // Fallback: Si no existe traducción, devolvemos la clave para identificar qué falta
            _ => messageKey
        };
}