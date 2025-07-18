# AxoMotor.ApiServer

Este es el back-end de nuestro proyecto AxoMotor.

## Cómo conectar con las bases de datos

Para funcionar, se requiere de una conexión con un servidor de MongoDB y MySQL/MariaDB. Para configurar las conexiones, se debe editar el archivo `appsettings.json` y establecer los valores requeridos en los siguientes campos: 

- `MongoDB.ConnectionUri`: URI de MongoDB, por ej. `mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000`
- `MySQL.ConnectionString`: String en formato `server=localhost;user=USER;password=PASSWORD;database=DB`
- `MySQL.ServerVersion`: Versión numérica de la base de datos, se puede obtener desde PhpMyAdmin.
