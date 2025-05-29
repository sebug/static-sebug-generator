# Static Sebug Generator
A static site generator for my blog.

Uses dotnet user secrets for the path of the sources and the path of the output
directory. Maybe I'll even add SFTP upload later, in which case the credentials
would also go there.

      dotnet user-secrets set "StaticSebugGenerator:SourceDirectory" /home/me/blog

The page is composed of main part, a heading and a navbar. Once I generate the static content I may start using CSS Grid.

