
SELECT 
    t."Id",
    t."Title",
    t."Description",
    t."Status",
    t."Priority",
    t."Duration",
    t."CreatedAt",
    t."ModifiedAt",
    COUNT(d."DependencyId") AS "NumberOfDependencies"
FROM "Tasks" t
LEFT JOIN "Dependencies" d ON t."Id" = d."TaskId"
GROUP BY t."Id", t."Title", t."Description", t."Status", t."Priority", t."Duration", t."CreatedAt", t."ModifiedAt"
ORDER BY t."CreatedAt" DESC;

SELECT 
    d."TaskId",
    d."DependencyId",
    t."Title" AS "TaskTitle",
    dep."Title" AS "DependencyTitle"
FROM "Dependencies" d
INNER JOIN "Tasks" t ON d."TaskId" = t."Id"
INNER JOIN "Tasks" dep ON d."DependencyId" = dep."Id"
ORDER BY d."TaskId";