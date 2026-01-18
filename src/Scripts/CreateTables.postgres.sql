
CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(500) NOT NULL,
    "Description" TEXT NOT NULL,
    "Status" VARCHAR(50) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "ModifiedAt" TIMESTAMP NOT NULL,
    "Priority" INT NOT NULL,
    "Duration" INT NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Tasks_Status" ON "Tasks" ("Status");
CREATE INDEX IF NOT EXISTS "IX_Tasks_CreatedAt" ON "Tasks" ("CreatedAt");

CREATE TABLE IF NOT EXISTS "Dependencies" (
    "TaskId" INT NOT NULL,
    "DependencyId" INT NOT NULL,
    CONSTRAINT "PK_Dependencies" PRIMARY KEY ("TaskId", "DependencyId"),
    CONSTRAINT "FK_Dependencies_Task"
        FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Dependencies_Dependency"
        FOREIGN KEY ("DependencyId") REFERENCES "Tasks" ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Dependencies_Task_Dependency"
ON "Dependencies" ("TaskId", "DependencyId");
