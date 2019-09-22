# MSC 2019 Freshman Portal System

Powered by ASP.NET Core 3.0 and Semantic UI

## Coding & Build

想用什么编辑器就用什么编辑器，以 vsc 举例，直接打开本项目文件夹即可  
需要安装 C# 插件  
如果使用 vs，则直接运行 FreshBoard.sln 即可

#### Restore Packages

```
dotnet restore
```

#### Build

```
dotnet build -r 目标系统 -c 发布类型
```

目标系统为系统平台和架构的组合，例如 win-x64、linux-x64 等  
发布类型可为 Debug 或 Release，默认为 Debug

## Run & Debug

vsc 可以直接按 F5 运行调试，注意打开项目文件夹的时候右下角的提示窗，需要选择 Yes 才可以

#### Run

```
dotnet run
```

注意运行此命令前，当前目录需要切换到项目根目录下，且需要将环境变量 DOTNET_ENVIRONMENT 设置为 'Development'  
设置方法：

```
Windows Powershell: $env:DOTNET_ENVIRONMENT = 'Development'
Windows Command Prompt: set DOTNET_ENVIRONMENT=Development
Linux Bash: export DOTNET_ENVIRONMENT=Development
```

不需要单独运行前端，直接使用此命令即可将网站运行在 http://localhost:5000 和 https://localhost:5001 （如果安装了证书） 上

## Publish

```
dotnet publish -r 目标系统 -c 发布类型
```

发布完成后会生成到 ./bin/发布类型/netcoreapp3.0/目标系统/publish 中  
复制 publish 里面的所有文件到目标机器上，运行 FreshBoard 主程序，网站将会自动启动并运行在 http://localhost:5000 以及 https://localhost:5001 （如果安装了证书），可使用 nginx 等做反向代理将其映射至 80/443 端口
