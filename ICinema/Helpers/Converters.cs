using ICinema.Models;
using ICinema.ViewModels.HelperModels;
using System;

namespace ICinema.Helpers;

public static class Converters
{
    public static List<OrdersModel> Convert(this List<Order> orders)
    {
        List<OrdersModel> profileOrders = new();

        foreach (var order in orders)
        {
            profileOrders.Add(order.Convert());
        }

        return profileOrders;
    }

    public static OrdersModel Convert(this Order order)
    {
        OrdersModel item = new OrdersModel();
        item.OrderId = order.OrderId;
        item.OrderStatus = order.OrderStatus;
        item.Price = order.Price;

        Session session = order.Tickets.First().Session;
        item.MovieName = session.Movie.MovieName;

        item.Date = session.Date;
        item.Time = session.Time;
        item.Tickets = order.Tickets.Select(x => x.Convert()).ToList();

        return item;
    }

    public static TicketModel Convert(this Ticket ticket)
    {
        TicketModel model = new();
        model.TicketId = ticket.TicketId;
        model.Price = ticket.Price;
        model.SeatId = ticket.SeatId;
        model.CreateDate = ticket.CreateDate;
        model.Session = ticket.Session.Convert();
        model.Seat = ticket.Seat.Convert();
        return model;
    }

    public static SeatModel Convert(this Seat seat)
    {
        SeatModel model = new();
        model.SeatId = seat.SeatId;
        model.RowNumber = seat.RowNumber;
        model.SeatNumber = seat.SeatNumber;
        model.SeatType = seat.SeatType;

        return model;
    }
    public static SessionModel Convert(this Session session)
    {
        SessionModel model = new();
        model.SessionTypeId = session.SessionTypeId;
        model.Date = session.Date;
        model.Time = session.Time;
        model.Format = session.Format;
        model.Hall = session.Hall.Convert();
        model.Movie = session.Movie.Convert();
        model.SessionType = session.SessionType;

        return model;
    }

    public static MovieModel Convert(this Movie movie)
    {
        MovieModel model = new();
        model.MovieId = movie.MovieId;
        model.MovieName = movie.MovieName;
        model.Genres = movie.Genres;
        model.Directors = movie.Directors;
        model.Writers = movie.Writers;
        model.Cast = movie.Cast;
        model.ReleaseDate = movie.ReleaseDate;
        model.Duration = movie.Duration;
        model.Description = movie.Description;
        model.IMDBRate = movie.IMDBRate;
        model.AgeRestriction = movie.AgeRestriction;
        model.PosterPath = movie.PosterPath;

        return model;
    }

    public static HallModel Convert(this Hall hall)
    {
        HallModel model = new();
        model.HallId = hall.HallId;
        model.HallName = hall.HallName;
//        model.Seats = hall.Seats.Select(x => x.Convert()).ToList();
        model.Technology = hall.Technology;

        return model;
    }
}
