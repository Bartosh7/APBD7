namespace APBD7.DTOs;

public record GetOrder(int IdOrder, int IdProduct, int Amount, DateTime CreateAt, DateTime FullFilledAt);