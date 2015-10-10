using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SignalR_Demo1.Models;

namespace SignalR_Demo1.Controllers
{
    public class GameHub : Hub
    {
        //public override Task OnConnected()
        //{
        //    // Add your own code here.
        //    // For example: in a chat application, record the association between
        //    // the current connection ID and user name, and mark the user as online.
        //    // After the code in this method completes, the client is informed that
        //    // the connection is established; for example, in a JavaScript client,
        //    // the start().done callback is executed.
        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected()
        //{
        //    // Add your own code here.
        //    // For example: in a chat application, mark the user as offline, 
        //    // delete the association between the current connection id and user name.
        //    return base.OnDisconnected(true);
        //}

        //public override Task OnReconnected()
        //{
        //    // Add your own code here.
        //    // For example: in a chat application, you might have marked the
        //    // user as offline after a period of inactivity; in that case 
        //    // mark the user as online again.
        //    return base.OnReconnected();
        //}


        public bool Join(string userName)
        {
            var player = GameState.Instance.GetPlayer(userName);

            if (player != null)
            {
                Clients.Caller.playerExists();
                return true;
            }

            player = GameState.Instance.CreatePlayer(userName);
            player.ConnectionId = Context.ConnectionId;
            Clients.Caller.name = player.Name;
            Clients.Caller.hash = player.Hash;
            Clients.Caller.id = player.Id;

            Clients.Caller.playerJoined(player);

            //return StartGame(player);
            return StartGameOnePlayer(player);
        }

        private bool StartGameOnePlayer(Player player)
        {
            if (player != null)
            {
                //Player player2;
                //var game = GameState.Instance.FindGame(player, out player2);
                //if (game != null)
                //{
                //    Clients.Group(player.Group).buildBoard(game);
                //    return true;
                //}

                //player2 = GameState.Instance.GetNewOpponent(player);
                //if (player2 == null)
                //{
                //    Clients.Caller.waitingList();
                //    return true;
                //}
                //var game = GameState.Instance.CreateGameOnePlayer(player);

                var game = GameState.Instance.CreateGameOnePlayer(player);
                game.WhosTurn = player.Id;

                //Clients.Caller.buildBoard(game);
                Clients.Group(player.Group).buildBoard(game);
                return true;
            }
            return false;
        }

        private bool StartGame(Player player)
        {
            if (player != null)
            {
                Player player2;
                var game = GameState.Instance.FindGame(player, out player2);
                if (game != null)
                {
                    Clients.Group(player.Group).buildBoard(game);
                    return true;
                }

                player2 = GameState.Instance.GetNewOpponent(player);
                if (player2 == null)
                {
                    Clients.Caller.waitingList();
                    return true;
                }
                game = GameState.Instance.CreateGame(player, player2);
                game.WhosTurn = player.Id;

                Clients.Group(player.Group).buildBoard(game);
                return true;
            }
            return false;
        }

        public bool Flip(string cardName)
        {
            var userName = Clients.Caller.name;
            var player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                Player playerOpponent;
                var game = GameState.Instance.FindGame(player, out playerOpponent);
                if (game != null)
                {
                    if (!string.IsNullOrEmpty(game.WhosTurn) && game.WhosTurn != player.Id)
                    {
                        return true;
                    }
                    var card = FindCard(game, cardName);
                    Clients.Group((string)player.Group).flipCard(card);
                    return true;
                }
            }
            return false;
        }

        public bool FlipOnePlayer(string cardName)
        {
            var userName = Clients.Caller.name;
            var player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                Player playerOpponent;
                var game = GameState.Instance.FindGameOnePlayer(player, out playerOpponent);
                if (game != null)
                {
                //    if (!string.IsNullOrEmpty(game.WhosTurn) && game.WhosTurn != player.Id)
                //    {
                //        return true;
                //    }
                    var card = FindCard(game, cardName);
                    Clients.Group((string)player.Group).flipCard(card);
                    //Clients.Caller.flipCard(card);
                    return true;
                }
            }
            return false;
        }

        private Card FindCard(Game game, string cardName)
        {
            return game.Board.Pieces.FirstOrDefault(c => c.Name == cardName);
        }

        public bool CheckCardOnePlayer(string cardName)
        {
            var userName = Clients.Caller.name;
            Player player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                Player playerOpponent;
                Game game = GameState.Instance.FindGameOnePlayer(player, out playerOpponent);
                if (game != null)
                {
                    if (!string.IsNullOrEmpty(game.WhosTurn) && game.WhosTurn != player.Id)
                        return true;

                    Card card = FindCard(game, cardName);

                    if (game.LastCard == null)
                    {
                        game.WhosTurn = player.Id;
                        game.LastCard = card;
                        return true;
                    }

                    //second flip
                    bool isMatch = IsMatch(game, card);
                    if (isMatch)
                    {
                        StoreMatch(player, card);
                        game.LastCard = null;
                        Clients.Group(player.Group).showMatch(card, userName);

                        //if (player.Matches.Count >= 16)
                        if (player.Matches.Count >= 30)
                        {
                            Clients.Group(player.Group).winner(card, userName);
                            GameState.Instance.ResetGame(game);
                            Clients.Group(player.Group).disconnected();
                            //StartGameOnePlayer(player);

                            return true;
                        }
                        return true;
                    }

                    //Player opponent = GameState.Instance.GetOpponent(player, game);
                    ////shift to other player
                    //game.WhosTurn = opponent.Id;

                    //Player opponent = GameState.Instance.GetOpponent(player, game);
                    //shift to other player
                    game.WhosTurn = player.Id;

                    Clients.Group(player.Group).resetFlip(game.LastCard, card);
                    game.LastCard = null;
                    return true;
                }
            }
            return false;
        }

        public bool CheckCard(string cardName)
        {
            var userName = Clients.Caller.name;
            Player player = GameState.Instance.GetPlayer(userName);
            if (player != null)
            {
                Player playerOpponent;
                Game game = GameState.Instance.FindGame(player, out playerOpponent);
                if (game != null)
                {
                    if (!string.IsNullOrEmpty(game.WhosTurn) && game.WhosTurn != player.Id)
                        return true;

                    Card card = FindCard(game, cardName);

                    if (game.LastCard == null)
                    {
                        game.WhosTurn = player.Id;
                        game.LastCard = card;
                        return true;
                    }

                    //second flip
                    bool isMatch = IsMatch(game, card);
                    if (isMatch)
                    {
                        StoreMatch(player, card);
                        game.LastCard = null;
                        Clients.Group(player.Group).showMatch(card, userName);

                        //if (player.Matches.Count >= 16)
                        if (player.Matches.Count >= 30)
                        {
                            Clients.Group(player.Group).winner(card, userName);
                            //GameState.Instance.ResetGame(game);

                            StartGameOnePlayer(player);

                            return true;
                        }
                        return true;
                    }
                    
                    //Player opponent = GameState.Instance.GetOpponent(player, game);
                    ////shift to other player
                    //game.WhosTurn = opponent.Id;

                    //Player opponent = GameState.Instance.GetOpponent(player, game);
                    //shift to other player
                    game.WhosTurn = player.Id;

                    Clients.Group(player.Group).resetFlip(game.LastCard, card);
                    game.LastCard = null;
                    return true;
                }
            }
            return false;
        }

        private void StoreMatch(Player player, Card card)
        {
            player.Matches.Add(card.Id);
            player.Matches.Add(card.Pair);
        }

        private bool IsMatch(Game game, Card card)
        {
            if (card == null)
                return false;

            if (game.LastCard != null)
            {
                if (game.LastCard.Pair == card.Id)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public bool RefreshGame(string userName)
        {
            var player = GameState.Instance.GetPlayer(userName);

            if (player != null)
            {
                Player playerOpponent;
                Game game = GameState.Instance.FindGameOnePlayer(player, out playerOpponent);
                GameState.Instance.ResetGame(game);
                return true;
            }

            return false;
        }
    }
}