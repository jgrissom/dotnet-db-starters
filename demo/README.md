# demo/ — your instructor's code, pushed during class

From week 8, the instructor builds part of the lab's station live in class and pushes each finished file here, in a folder for that week (`demo/week-08/`, …). You copy it into your own project at the start of the matching lab task, and read and run it while you work. The lab README gives the exact commands.

- **You never edit these files** — they're the instructor's, so copying one in can never overwrite your work.
- **A folder starts empty and fills up during that week's class.** Working at home after class? The files are already here.

## For the instructor: reset once per term

Empty every week's folder before the next term's first class (or at the end of this one), so the files arrive live again:

```bash
git -C ../dotnet-db-starters rm -r --quiet demo/week-*
git -C ../dotnet-db-starters commit -m "demo: reset for the new term"
git -C ../dotnet-db-starters push
```

This `README.md` stays. If the reset is forgotten, nothing breaks — students simply have the files before they're built.
