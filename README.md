bugtable
========
Build
-----
Use
``` bash
dotnet build
```

Howto
-----
Creates a table from a list of ADO bugs

To get details of bugs with ID nnnn, mmmm and kkkk use
``` bash
bugtable nnnn mmmmm kkkk
```
Or directly in the source folder
``` bash
dotnet run nnnn mmmm kkkk
```

This generate the following table in a file named workitems.html


|ID     |Title       |State       |Assigned To     |Notes                  |
|-------|------------|------------|----------------|-----------------------|
| 1234  |Awesome bug | Active     |Ford Prefect    | Lets fix it           |
| 4242  |Meh bug     | Resolved   |Arthur Dent     | Lets fix it           |


Pre-reqs
--------
Standard setup of vscode, .NET SDK
ADO access token in the environment variable
```bash
AZDO_PAT_SC2020
```
