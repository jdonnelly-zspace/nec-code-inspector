"""Compile the project's scripts against an installed Unity editor and report real errors.

    python tools/unity-compile-check/check.py [path to Unity Editor/Data folder] [--list]

Needs the .NET 8 SDK and a Unity editor install (the default path is Unity 6000.4.1f1 in the Hub).

Why it works this way: the C# compiler does not check method bodies while there are errors in
declarations, and several packages are not installed here (zSpace SDK, PrimeTween, Localization), so
files that use them cannot compile. The tool therefore leaves out every file that needs a missing type,
and every file that needs one of those files, until what remains compiles on its own, then checks that
remainder fully (method bodies included). uGUI and TextMeshPro are stand-ins (Stubs.cs). Files left out
are listed with --list; they still need a real Unity compile. This does not replace opening the project
in Unity.
"""
import os
import re
import shutil
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.abspath(os.path.join(HERE, "..", ".."))
PROPS = os.path.join(HERE, "excluded.props")
MISSING = {"CS0246", "CS0234", "CS0103"}  # type or namespace not found, name does not exist
MAX_ROUNDS = 25


def write_props(excluded):
    items = "\n".join(f'    <Compile Remove="$(Repo){p}" />' for p in sorted(excluded))
    with open(PROPS, "w", encoding="utf-8") as f:
        f.write(f"<Project>\n  <ItemGroup>\n{items}\n  </ItemGroup>\n</Project>\n")


def build(dotnet, extra):
    args = [dotnet, "build", os.path.join(HERE, "UnityCompileCheck.csproj"), "-nologo", "-v", "q"] + extra
    run = subprocess.run(args, capture_output=True, text=True, encoding="utf-8", errors="replace")
    errors = {}
    for line in run.stdout.splitlines():
        m = re.match(r"(.+?)\((\d+),\d+\): error (CS\d+): (.*?)(?: \[.*)?$", line.strip())
        if m:
            errors[(m.group(1), m.group(2), m.group(3), m.group(4))] = line
    return run, sorted(errors)


def main():
    argv = [a for a in sys.argv[1:] if a != "--list"]
    show_list = "--list" in sys.argv
    dotnet = shutil.which("dotnet") or "C:/Program Files/dotnet/dotnet.exe"
    extra = ["-p:UnityData=" + argv[0].replace("\\", "/").rstrip("/") + "/"] if argv else []

    excluded = set()
    real = []
    for _ in range(MAX_ROUNDS):
        write_props(excluded)
        run, errors = build(dotnet, extra)

        if not errors and run.returncode != 0:
            print(run.stdout[-1500:] or run.stderr[-1500:])
            print("The build failed before compiling (is the Unity path right?).")
            return 2

        missing_files = {e[0] for e in errors if e[2] in MISSING}
        real = [e for e in errors if e[2] not in MISSING]
        if not missing_files:
            break
        excluded |= {os.path.relpath(p, REPO).replace("\\", "/") for p in missing_files if os.path.exists(p)}
    else:
        print("Gave up: the set of files left out did not settle.")
        return 2

    scripts = sum(1 for r, _, fs in os.walk(os.path.join(REPO, "Assets", "_Project", "Scripts")) for f in fs if f.endswith(".cs"))
    print(f"{scripts - len(excluded)} of {scripts} script files checked in full; {len(excluded)} left out (they need packages not installed here)")
    print(f"{len(real)} real compiler errors")
    for path, line, code, msg in real:
        print(f"  {os.path.relpath(path, REPO).replace(chr(92), '/')}({line}): {code} {msg}")
    if show_list:
        print("Left out:")
        for p in sorted(excluded):
            print("  " + p.replace("../../", ""))
    return 1 if real else 0


if __name__ == "__main__":
    code = main()
    if os.path.exists(PROPS):
        os.remove(PROPS)
    sys.exit(code)
