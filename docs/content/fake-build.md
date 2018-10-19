# FAKE build

ProjectScaffold uses the [FAKE](https://fake.build/) build tool to automate the complete build and release process for a application/solution.

The `build.fsx` file contains this logic and is written in FAKE's build DSL. It contains a number of common tasks (i.e. build targets) such as directory cleaning, unit test execution, and NuGet package creation. You are encouraged to adapt existing build targets and/or add new ones as necessary. 

However, if you are leveraging the default conventions, as established in this scaffold project, you can start by simply supplying some values at the top of this file. They are as follows:

  - `project` - The name of your project, which is used in serveral places: 
    - the generation of AssemblyInfo.
    - the name of a NuGet package.
    - naming/finding a directory under `src`.
  - `summary` - A short summary of your project, used as:
    - the description in AssemblyInfo. 
    - the short summary for the NuGet package.
  - `description` - A longer description of the project used as a description for your NuGet package.
  - `authors` - A list of author names, to be displayed in the NuGet package metadata.
  - `tags` - A string containing space-separated tags, to be included in the NuGet package metadata.
  - `solutionFile` - The name of your solution file (sans-extension). It is used as part of the build process.
  - `testAssemblies` - A list of [FAKE](https://fake.build/) globbing patterns to be searched for unit-test assemblies.
  - `gitHome` - The base URL / user profile hosting this project's GitHub repository. This is used for publishing documentation.
  - `gitName` - The name of this project's GitHub repository. This is used for publishing documentation.

## Running targets 

You can run any target in the build script using:

    $ build.cmd [TARGETNAME] // on windows
    $ build.sh [TARGETNAME] // on mono
    
If you don't specify a target name it will run the default process, which includes running tests and [building docs](writing-docs.html).
    
More details can be found in the [FAKE](https://fake.build/) docs.
