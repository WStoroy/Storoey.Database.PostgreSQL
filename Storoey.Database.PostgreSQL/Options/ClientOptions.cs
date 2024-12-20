using System.ComponentModel.DataAnnotations;

namespace Storoey.Database.PostgreSQL.Options;

/// <summary>
///     Represents configuration options required to establish a connection to a PostgreSQL database.
/// </summary>
/// <remarks>
///     The options include details such as the host, port, database name, user credentials,
///     and an identifier for the machine. All properties are required to ensure a proper connection configuration.
/// </remarks>
public sealed record ClientOptions
{
    /// <summary>
    ///     Gets or initializes the host address of the PostgreSQL database server.
    /// </summary>
    /// <remarks>
    ///     This property specifies the network address (hostname or IP address) of the PostgreSQL server
    ///     that the application will connect to. It is a required field and must be correctly configured
    ///     for establishing a database connection.
    /// </remarks>
    [Required]
    public required string Host { get; init; }

    /// <summary>
    ///     Gets or initializes the port number used to connect to the PostgreSQL database server.
    /// </summary>
    /// <remarks>
    ///     This property specifies the network port for communicating with the PostgreSQL server.
    ///     The default value is 5432, which is the standard port for PostgreSQL. It must be correctly
    ///     configured to establish a successful connection to the database.
    /// </remarks>
    [Required]
    public required int Port { get; init; }

    /// <summary>
    ///     Gets or initializes the name of the PostgreSQL database to connect to.
    /// </summary>
    /// <remarks>
    ///     This property defines the specific database within the PostgreSQL server
    ///     that the application will interact with. It must be correctly specified
    ///     to ensure the intended operations are performed within the correct database context.
    /// </remarks>
    [Required]
    public required string Database { get; init; }

    /// <summary>
    ///     Gets or initializes the username used for authenticating against the PostgreSQL database server.
    /// </summary>
    /// <remarks>
    ///     This property specifies the username credential required to establish a connection to the PostgreSQL database.
    ///     It is a mandatory field and must correspond to a valid user account on the database server.
    /// </remarks>
    [Required]
    public required string Username { get; init; }

    /// <summary>
    ///     Gets or initializes the password for authenticating the connection to the PostgreSQL database.
    /// </summary>
    /// <remarks>
    ///     This property specifies the credential password for the database user. It is required to authenticate
    ///     and establish a secure connection to the PostgreSQL server. The password must be kept confidential and managed
    ///     securely.
    /// </remarks>
    [Required]
    public required string Password { get; init; }

    /// <summary>
    ///     Gets or initializes the identifier for the machine generating unique identifiers.
    /// </summary>
    /// <remarks>
    ///     This property specifies a unique machine identifier utilized by the SnowflakeID generator
    ///     to ensure the creation of unique IDs across distributed systems. Its value must be configured
    ///     appropriately for the environment to avoid ID collision issues.
    /// </remarks>
    [Required]
    public required int MachineId { get; init; }
}