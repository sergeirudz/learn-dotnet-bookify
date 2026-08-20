namespace Bookify.Domain.Apartments;

/**
 * Address record. Identified by its values.
 * It has structural equality, Record supports. Immutability, record supports.
 */
public sealed record Address(
    string Country,
    string State,
    string ZipCode,
    string City,
    string Street);
