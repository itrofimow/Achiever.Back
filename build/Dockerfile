FROM microsoft/dotnet:2.2-sdk as builder
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /published Achiever.Web/Achiever.Web.csproj


FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app

COPY --from=builder /published .

RUN apt-get update && apt-get install -y libfontconfig1

ENTRYPOINT ["dotnet", "Achiever.Web.dll"]
