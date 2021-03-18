FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
RUN cd /usr/local/src 
WORKDIR /app
COPY . . 
EXPOSE 80 
ENTRYPOINT ["dotnet", "Bbin.ManagerWebApp.dll"] 