using Ipfs;
using Ipfs.Engine;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ipfs_demo
{
    public class Program
    {
        Peer local;
        public async Task Invoke()
        {
            var ctx = new CancellationTokenSource();

           const string passphrase = "this is not a secure pass phrase";
            var ipfs = new IpfsEngine(passphrase.ToCharArray());
             ipfs.PubSubService.Start();
            await ipfs.StartAsync();

            var addr =  await ipfs.Swarm.AddAddressFilterAsync(MultiAddress.TryCreate("/ip4/104.131.131.82/tcp/4001/p2p/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ"));
            await ipfs.Swarm.ConnectAsync(addr);
            
            await ipfs.PubSub.SubscribeAsync("furesoft", handler, ctx.Token);

            while (true)
            {
                Console.Write("-> ");
                var msg = Console.ReadLine();

                if(msg == "/stop")
                {
                    await ipfs.StopAsync();
                    return;
                }

                await ipfs.PubSub.PublishAsync("furesoft", msg);

            }
        }

        public static void Main()
        {
            new Program().Invoke().Wait();
        }

        private void handler(IPublishedMessage obj)
        {
            var sender = obj.Sender;
            if (local == sender) return;

            Console.WriteLine(obj.Sender.Id.ToBase58() + ": " + Encoding.ASCII.GetString(obj.DataBytes));
        }
    }
}
