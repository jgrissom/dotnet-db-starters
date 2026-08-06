# .NET Database Development — Weekly Starters

**This repo is the code you copy. Nothing here is reading material, and nothing here gets edited in place.**

Each folder is one week, ready to go:

```
week-01/
├─ Lab/                 the in-class lab — never collected, worth 0 points
├─ Lab.Checks/          the lab's checks — read-only
├─ Homework/            your homework's starting point
└─ Homework.Checks/     the homework's checks — read-only
```

## Every week, three steps

**1. Pull, so you have this week's folder:**

```bash
git pull
```

**2. Copy the week's folder into your coursework repo** — in Finder or File Explorer, copy `week-02` (say) and paste it into `dotnet-db-coursework`. **Copy it; don't move it** — this clone keeps its own copy.

**3. Work on your copy.** Everything you do happens in `dotnet-db-coursework`, in the one VS Code window you keep all semester:

```bash
dotnet test week-02/Lab.Checks
```

> [!CAUTION]
> **Never open or edit anything inside this clone.** It's a delivery box. If you write code in here, it isn't in your repo, it isn't backed up, it isn't graded — and the next `git pull` may overwrite it.

## Where everything else lives

**The course itself — labs, homework, lecture notes, slides — is in [dotnet-db-dev](https://github.com/jgrissom/dotnet-db-dev)**, and you read it in your browser. Slides are also published at **https://jgrissom.github.io/dotnet-db-dev/**.

You submit through Canvas: the URL of your own **private** `dotnet-db-coursework` repo.
