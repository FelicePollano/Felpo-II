﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FelpoII.Interfaces;
using FelpoII.Pieces;

namespace FelpoII
{
    public abstract class Piece:IPiece
    {
        public  const int NORTH = 16;
        public  const int NN    = NORTH + NORTH;
        public  const int SOUTH = -16;
        public  const int SS  =   SOUTH + SOUTH ;
        public  const int EAST = 1;
        public  const int WEST = -1;
        public  const int NE  =  17;
        public  const int SW  =  -17;
        public  const int NW  =  15;
        public  const int SE = -15;

        public ulong ZKey
        {
            get { return GetZKey(HomeSquare); }
        }

        public abstract ulong GetZKey(int square);

        // Zobrist key raw array
        
        public static ulong[] Random64 = new ulong[781]{
                0xA7B9F615869D33E0UL,0x3A0633F58D79676EUL,0xEA62DE57AB7E8BC7UL,0x6B4A16EF4B024CBEUL,
                0x3BDC114D0993F7CAUL,0xB977C51803B62985UL,0xDBC00303372B9743UL,0x5207D3A2C3A96D45UL,
                0x9FDCA0746D991BFEUL,0xA34A344A34CBC580UL,0x4DECBB22302D3047UL,0x6813278F1F6FC4F9UL,
                0x1C5FBF1A8690EBA0UL,0x1D7316A94168F2D5UL,0xEA60561F3E0F028UL,0x5F71B32922EB594DUL,
                0x293AA00C2BDAF664UL,0xE5767C49F21B076BUL,0x97083600C5D41382UL,0x973B12A7C9348102UL,
                0xD666E2A192F0FA8DUL,0xE0911B591E6EFDC6UL,0xAFFD5F026E1EA4B7UL,0x7C9971C10DBF9D07UL,
                0x4ADD750E72F394DFUL,0xCC43DFF8CB70D403UL,0xB0A2A68B6DBE14E4UL,0xDA3C4C68CD8599C2UL,
                0xCB7E501C2B5D92F0UL,0xBFAF5233BA93109UL,0xE46FBF90444322C7UL,0x8BE376115E36B9ADUL,
                0x8083BF28D9553FD9UL,0x8088B56F13C5735UL,0x25C751852265F51FUL,0xEDFCD41457D332FUL,
                0xF1A544F6F18E1507UL,0x389ADD3346DD8A15UL,0x8FC15A78A0A603AEUL,0x8A43CF14D1EF982AUL,
                0xC11DE27063494CBCUL,0xA37A29C6D6238354UL,0x84766B45C5D9D3EUL,0x6DAA8676DF401C14UL,
                0x75AD7119A6D6704FUL,0x5348BD749184C2F9UL,0x1A9641C04053F21AUL,0xCAC2DA0ED1D482CCUL,
                0xE0496D448D88E265UL,0x62CD6A0655F2D38BUL,0x647720525E1551B0UL,0x379C4785227E451FUL,
                0x66E14C65C8DE1080UL,0x54E82FF4E1676F8EUL,0xDA671B2386CB31FCUL,0xD7E9A6F2BA9294FEUL,
                0x2380BD0E90304D85UL,0x5E953D34D3B9DF7UL,0x65E7D5B10FFF22D7UL,0xC27DCB27007DF81AUL,
                0x96BDA56FE55A908AUL,0x8A1FC2499FB74A50UL,0x8F649BBE40490E57UL,0x1C4F503EB7CBFEDUL,
                0x5CE8E4E748C99C80UL,0x5BE7314C9C86D9E2UL,0x1DF735DC60E470BDUL,0xAB2C2D65727934EFUL,
                0x9B28A5A927C6A54BUL,0x295A87F0635E8A8BUL,0x76367809F9B05C79UL,0x4BE18C145ED48DF6UL,
                0x990EC4E7A398DE52UL,0xD4C0C6A943135642UL,0xDC9D00D52818BE31UL,0x6022E93D7B0C64A5UL,
                0xF072772BF547510EUL,0xB2E1432EFBF3D24EUL,0xA5A94468457728A1UL,0xD20052875A5E075CUL,
                0x55C420DA601B408BUL,0x6264484CF7608628UL,0xD6304B37F8B170DFUL,0x1A3BEB3C62B3148UL,
                0x18648BFB9B03568DUL,0xE839B6D11EC94573UL,0x4529EDBD950CA9D8UL,0x9A147F34FBF264F7UL,
                0xCAAEC6696C210817UL,0xB9E98690C46F5A38UL,0xB115FE9176E1D34EUL,0xD4E942AB0ACDEED4UL,
                0xD5FD7F3A1BA3E21AUL,0x5DB104135FC55666UL,0xB3BB1B32E074376EUL,0x73D3238FAA9C7D23UL,
                0xC867740BB5F9A806UL,0xAA5B0E8077E3C6F5UL,0x6A4D357D4B1ED8C5UL,0xC7F4FF9A4D5AD825UL,
                0x8F380E8E99B4491DUL,0x4E8BBF55C72892DFUL,0xD55CF2D6AB32DE2EUL,0xE62438DC10167B71UL,
                0xABDE04AE1499F03FUL,0x1DA5D1F3CCBD968FUL,0xF438F5D1657409C8UL,0xEA62CE55CA22FA0FUL,
                0x7E5210232303DB9UL,0xB98A8925E5ECF5B7UL,0x7A6F3FA5FC2F0121UL,0xA10F1B0C4FC3E66DUL,
                0xEF0722F9739F5A94UL,0x44018976CE6CF861UL,0x1D551475432EE57BUL,0x44917277DD753AD9UL,
                0x1E018B0733926527UL,0x1ABAEA0193EB84C0UL,0xF5F019307A267CA0UL,0x1F87023B4B3DA932UL,
                0x76A9D99DF84912B6UL,0xCAA2A98286E60F35UL,0x4AE28459BE537F80UL,0x202DC161848D7B5BUL,
                0x6077DA5735AAE442UL,0x4114E8AA156B0B3BUL,0xA528AB73F7CEAB12UL,0xEB45432D5FC83C64UL,
                0xFC9F5F817C7B07B0UL,0x71A7AB1EF967DA8EUL,0x708880384891EAABUL,0xBC667181212155BDUL,
                0x82C9C31798D8CF03UL,0x9341EB5089063D8EUL,0x1F0669DF5FF5128DUL,0x3718A4F8BEF209DAUL,
                0xEA19AF48AC001B6FUL,0x58F71D26A2253240UL,0x97170BE1FE8CDBD0UL,0x2674C47BF6FD6A66UL,
                0xCE1A8B8715E0EEUL,0xB9B56561210C362UL,0x74C24FEBE3391AC9UL,0xDA7D6A0F42E72397UL,
                0x6F5563D8E9FBAC87UL,0xDA8540B66702FC44UL,0x71E160556F6F2BA5UL,0x7C0AABC6D3989720UL,
                0x39D4375E56BE9DF7UL,0x8BAE36535C4E1727UL,0x4D45A759D8BCCBC8UL,0xE739D1CC01AA4645UL,
                0x8ADCBA53C193C82EUL,0xF545633A3360722EUL,0xACBC19A1A8ECA489UL,0x12F1C97E8FE264A2UL,
                0x46E9B9A9BDDA89AFUL,0xE2BB839A3833CC1UL,0xD4F42F2454AC77FBUL,0xCF13AAA6EF308A8UL,
                0x202BBF4B0781388AUL,0x295975A488574EF1UL,0xE8BE7FC53D35B2A0UL,0xD044A2D65C4435DAUL,
                0x3C532D91534DAD51UL,0x6E69C46401042CE4UL,0x74DD0F554266863AUL,0xCA1B18F42CF7494FUL,
                0x480C0943E32F7C1DUL,0x358414AA76E5FE04UL,0x25757706120D4858UL,0x48771B4E1039D1CCUL,
                0x2979F9E5E9C9B876UL,0x35B0332842A36894UL,0x53D154738EF35A74UL,0xE36A2A2B02529F93UL,
                0x689FED28BBF40311UL,0x1963FBD88E4E9210UL,0xF7F6D4A0E3D0D0A9UL,0xCB4ADFC909B95007UL,
                0x85E8C2C5589965E6UL,0xEA43E3E77DD845B8UL,0x70107C33AEC1B79DUL,0x74D26EF735D9934FUL,
                0x679D94F3E68A22AEUL,0x70608759A85220C2UL,0xFFC79E4A453CECFAUL,0x565EEEF053BF6639UL,
                0x20120C51562778F3UL,0x917D0319298F5F22UL,0x219283D79CE872FFUL,0xFC315EA094D07835UL,
                0x3822CBF05C29A997UL,0x3E6E43D6D3A3749FUL,0x5FA2157E740A8681UL,0x152A69E53C5ADC44UL,
                0x6D08C736F2970D82UL,0x9E60ACE50706FBCDUL,0x2A9E304C979BDA1DUL,0x1A2F8A97573D8663UL,
                0xC862117890FD9225UL,0xD135A45AD5A22194UL,0x12D0FD18B44DED59UL,0x76D8ED0D771EAFECUL,
                0x98ECB89DD9AA41A6UL,0xAF1073BC5A0D3529UL,0x74D931A05CDE4C10UL,0x704640E1C5B65F03UL,
                0xD3EBF61FD4BFBAD2UL,0xDE4456DBF642EEB8UL,0xCF1742BA535B26CEUL,0xCCF75D7E4FD401A3UL,
                0xCF738C1555FF3C0UL,0x1596158C865845FUL,0x2A5F0C978FC3959EUL,0x2A1D78DCA67E3B8DUL,
                0x7ED5E569A45CE426UL,0x9D70C683FCD8A6A0UL,0xF0C14AC3AAF13E65UL,0xE809B30450FB8A84UL,
                0x56864BDF89E98A2DUL,0x1AB40A2E8E5DBBFAUL,0x8ED4C45A3CAF10DEUL,0x57D469316060774BUL,
                0xD263BAF3EEB9BE48UL,0x70ED06959998441UL,0xCD073AA46DC6C3B9UL,0xE95853056AF2CC4FUL,
                0x5C37E3426F29918AUL,0x995D5FD3F6869990UL,0x228317C4D75EA901UL,0x4B25F6C8C69B80DBUL,
                0xF2F56413E0997C71UL,0x87D92AF7763FE510UL,0xF49F1019E6F8F089UL,0x75E1296408A66618UL,
                0xA828813A416B850EUL,0x6549020BA7D13EFAUL,0xE4B856EFA5FA9EF9UL,0x3B01726B7622847UL,
                0xFF2A1920144D2AFFUL,0x1D4CB20F0856EB68UL,0x68C0B1A8513F18CAUL,0x3D69C50B7A6367AFUL,
                0xC82393473F8CCD82UL,0xA70A162154701951UL,0xA3C400418DD50B81UL,0xC4C0F641E08968BUL,
                0x4987B5E5CFB349F0UL,0x949EAEC49E2AD230UL,0xFE5436314C95F158UL,0x6CFA4D0C555A884BUL,
                0xB32ED7284E6534C3UL,0x8305EEEF0ACD8748UL,0x8172070653822F55UL,0xD1483652594AE089UL,
                0x958EB4CB1C7C35F2UL,0x953EED613D5CCDB4UL,0xE865C807BAFB2911UL,0xF636BBA256DB5BE9UL,
                0x9F3737DA7D27A477UL,0x90C62329F935CF72UL,0x8B0B3B8EE218B8BDUL,0x456470136E8C87FFUL,
                0x10CE3C89F9D4C216UL,0xEC64BAFC448951E8UL,0xF7DE6989150626D7UL,0xC54A7DDA6FFC0047UL,
                0x227F038C906300EBUL,0xE7C0752C6A10ED4CUL,0x4847C74337DDAF68UL,0x97C47D949A41121BUL,
                0x94320EFFECB9F81DUL,0xB6490D849253D79CUL,0xD32F7F28407C3DDDUL,0x2CC432525EC734B5UL,
                0x46949FB56389FBFFUL,0x8A8EA34BA2D995C7UL,0xFBC1585B7F976B25UL,0xD0970B172B1B895CUL,
                0xA922754A4B2487D0UL,0x187140C37BD3FAA6UL,0xFAA3440909F655AEUL,0x38F0A3413103A0A7UL,
                0xAB931E0C2F01120DUL,0xB123E3E30DCEC5DEUL,0xF9694C8789D9733CUL,0xA9FDF73AE3B3017BUL,
                0xDA616C2252F93C2CUL,0x12541F3DB70AFE02UL,0x32085006FB81B1EUL,0x25E1CC3603B8F22AUL,
                0x4B28B1CB5C27D7ABUL,0x909A0F4EDC8FD47EUL,0xB7CF3B06B7FCAD16UL,0x9AF9DE785F6D52FFUL,
                0xC20A3F00CC549E7UL,0x692AAFB6E2FCD16FUL,0xFC4C3569C16F7D3UL,0xDD4257870FE5AD3FUL,
                0xFF6111A5BE8FE787UL,0x1E7CAD36DBC7CABUL,0xC2B4225DFEEF50AEUL,0x3A6A29F402CEDD29UL,
                0x555392DDD42ABC18UL,0x1E7962CECABA4D79UL,0x3605BC8624047098UL,0x4E0624B095D7F34FUL,
                0x545B05C3245666CBUL,0x24374451C76E1D8AUL,0xBCEE6B6FABBA5A50UL,0xB921B2AF97C8A0CCUL,
                0xBDDA89AF0F36CF91UL,0xA3833CC1B713E0C6UL,0x54AC77FB2A10DE12UL,0x6EF308A8D3C03680UL,
                0x781388AAFBF54D7UL,0x88574EF1BEB1D445UL,0x3D35B2A0C4F86C61UL,0x5C4435DAEFA49E5FUL,
                0x534DAD5144534BB0UL,0x1042CE4BD9E44A6UL,0x4266863A5170B112UL,0x2CF7494F969B157AUL,
                0xE32F7C1D2410F7F5UL,0x76E5FE04A7FA341AUL,0x120D4858F000FC41UL,0x1039D1CC1FFC94CFUL,
                0xE9C9B876A5CAF5D8UL,0x42A368940E6CA422UL,0x8EF35A742730B9F9UL,0x2529F9313EF8438UL,
                0xBBF40311A0D7B620UL,0x8E4E9210B6E5A537UL,0xE3D0D0A97510DFA2UL,0x9B95007F9A72653UL,
                0x589965E6EF55B0F0UL,0x7DD845B80543104CUL,0xAEC1B79D0AD924F0UL,0x35D9934F88720266UL,
                0xE68A22AEA6F947DFUL,0xA85220C24F9C3BD3UL,0x453CECFAB79E28C8UL,0x53BF66396F18729CUL,
                0x562778F3741ACB4DUL,0x298F5F22A42705AEUL,0x9CE872FFB6C5D313UL,0x94D078357143BF4AUL,
                0x5C29A997E38C443EUL,0xD3A3749FDB8C2186UL,0x740A8681888DC53EUL,0x3C5ADC441027B185UL,
                0xF2970D8204C6AC20UL,0x706FBCD1D121EBBUL,0x979BDA1D523E8F64UL,0x573D866336CEB1A2UL,
                0x90FD92251990F2E8UL,0xD5A22194092D9481UL,0xB44DED5937AAC82AUL,0x771EAFEC50DF1261UL,
                0xD9AA41A6CCB36836UL,0x5A0D352950655968UL,0x5CDE4C10D3E95C64UL,0xC5B65F03E23A1505UL,
                0xD4BFBAD2D52F6024UL,0xF642EEB819F60F4EUL,0x535B26CE4DE1D97BUL,0x4FD401A3EA2EDAF8UL,
                0x555FF3C0B6B3919AUL,0xC865845F489029C5UL,0x8FC3959E74CFE8F4UL,0xA67E3B8D518240DAUL,
                0xA45CE42644556EFBUL,0xFCD8A6A0FFE6B788UL,0xAAF13E65D687F61FUL,0x50FB8A84A372ECA2UL,
                0x89E98A2D748504F6UL,0x8E5DBBFAF6D9D62EUL,0x3CAF10DE38C76B75UL,0x6060774BE340C02CUL,
                0xEEB9BE482FDE0B3CUL,0x5999844166B6DA47UL,0x6DC6C3B97A93EAE5UL,0x6AF2CC4FF00E3A46UL,
                0x6F29918A808B6709UL,0xF68699900E9F89CBUL,0xD75EA901B0B183A1UL,0xC69B80DBF634C254UL,
                0xE0997C712EA1BD83UL,0x763FE510333902DAUL,0xE6F8F089C4367B37UL,0x8A6661885A1261BUL,
                0x416B850E688AC5E1UL,0xA7D13EFADF97D007UL,0xA5FA9EF9682B3B11UL,0xB7622847C06DAC08UL,
                0x144D2AFF4F21B2CBUL,0x856EB68CAD53691UL,0x513F18CA1D6A1287UL,0x7A6367AFF69C13D3UL,
                0x3F8CCD82EBA882E2UL,0x54701951D7DB47C4UL,0x8DD50B81918F9AEEUL,0x1E08968B6460E6F2UL,
                0xCFB349F0205EED63UL,0x9E2AD2303839391FUL,0x4C95F1581AC435EFUL,0x555A884B519F146CUL,
                0x4E6534C3F25560E0UL,0xACD87486E4C166FUL,0x53822F55949A9D32UL,0x594AE089546343CFUL,
                0x1C7C35F23BCBCBBAUL,0x3D5CCDB45728F444UL,0xBAFB291144F57323UL,0x56DB5BE91E9A26B7UL,
                0x7D27A477D1494C05UL,0xF935CF7222851434UL,0xE218B8BD8C955C37UL,0x6E8C87FFD1E3E16AUL,
                0xF9D4C2166F153A25UL,0x448951E84DCF068EUL,0x150626D7ABAE5C2BUL,0x6FFC00470C51C736UL,
                0x906300EB23188E7CUL,0x6A10ED4CF62C3D5FUL,0x37DDAF681C6B7E6FUL,0x9A41121B5767AF55UL,
                0xECB9F81DFC00E2D7UL,0x9253D79C7C2A581BUL,0x407C3DDD9CBC14B3UL,0x5EC734B55B097AB6UL,
                0x6389FBFFD2789AD5UL,0xA2D995C7A50A8F8CUL,0x7F976B25EB2BD0DCUL,0x2B1B895CB0916572UL,
                0x4B2487D058C09A53UL,0x7BD3FAA634D40426UL,0x9F655AE5AF7D0E5UL,0x3103A0A75488D44FUL,
                0x2F01120DFB198BBFUL,0xDCEC5DEC43B3715UL,0x89D9733C52FE5699UL,0xE3B3017B44AFF50CUL,
                0x52F93C2C7D20FA63UL,0xB70AFE02E8097BF8UL,0x6FB81B1EF06552C7UL,0x3B8F22A66A0555FUL,
                0x5C27D7AB0A1CB180UL,0xDC8FD47EC6F8C263UL,0xB7FCAD166CAC8AA3UL,0x5F6D52FFEF063BACUL,
                0xCC549E7EEF93308UL,0xE2FCD16F61C2B315UL,0x9C16F7D34CE4E45BUL,0xFE5AD3F7074AA6FUL,
                0xBE8FE78726468FB1UL,0x6DBC7CAB4F4F7C9EUL,0xFEEF50AE76EB07BDUL,0x2CEDD2932B7D34EUL,
                0xD42ABC18C500F659UL,0xCABA4D79803E450AUL,0x24047098BEED4DC9UL,0x95D7F34FCF2952FDUL,
                0x245666CB68CCED05UL,0xC76E1D8AA5D0A284UL,0xABBA5A50001C1C9EUL,0x97C8A0CCD84DBF46UL,
                0xF36CF910F36CF91UL,0xB713E0C6AE023ED3UL,0x2A10DE12FFB93AC3UL,0xD3C036800CB235E2UL,
                0xAFBF54D7500CCBF6UL,0xBEB1D445E029CD7BUL,0xC4F86C615D9FD12EUL,0xEFA49E5FB5085734UL,
                0x44534BB03578AAC7UL,0xBD9E44A6E47D3E4FUL,0x5170B112F7189E70UL,0x969B157A4CACD0F1UL,
                0x2410F7F589184015UL,0xA7FA341A4CEF388BUL,0xF000FC4121228726UL,0x1FFC94CFBA2464E5UL,
                0xA5CAF5D8A7A650BDUL,0xE6CA42230BE9D6EUL,0x2730B9F947263C9DUL,0x13EF8438FF061B53UL,
                0xA0D7B6205594D0D9UL,0xB6E5A5379B512AD7UL,0x7510DFA2E78E3795UL,0xF9A726537A3FED19UL,
                0xEF55B0F090350861UL,0x543104CF3D9CFCCUL,0xAD924F07B4197C5UL,0x88720266E9303362UL,
                0xA6F947DFE7D5F427UL,0x4F9C3BD38B889158UL,0xB79E28C8F594A00FUL,0x6F18729C6CCA64B2UL,
                0x741ACB4DD6230B6FUL,0xA42705AE053B0711UL,0xB6C5D313CCDC12A4UL,0x7143BF4A269B2F10UL,
                0xE38C443E17EDE354UL,0xDB8C2186EF8C9136UL,0x888DC53E6AA328ABUL,0x1027B18519526652UL,
                0x4C6AC206E5692B6UL,0x1D121EBB747A91F2UL,0x523E8F64EFB8278AUL,0x36CEB1A2C64EB829UL,
                0x1990F2E80B8700A0UL,0x92D94811F2BD116UL,0x37AAC82ACF4C3704UL,0x50DF126154AE33A7UL,
                0xCCB36836A1D9CE44UL,0x50655968A4CF34AFUL,0xD3E95C64C7D78C5FUL,0xE23A1505DD5BE873UL,
                0xD52F60246DCF7A3DUL,0x19F60F4EECCF4DFCUL,0x4DE1D97BCEBB6F86UL,0xEA2EDAF84B836346UL,
                0xB6B3919A8CED3DFDUL,0x489029C5F4307522UL,0x74CFE8F4C972F3D7UL,0x518240DA39574610UL,
                0x44556EFBB4E61400UL,0xFFE6B7884958989EUL,0xD687F61F02419D1CUL,0xA372ECA2236C129CUL,
                0x748504F6113A90CCUL,0xF6D9D62E71A24DDCUL,0x38C76B75ACA7CBFBUL,0xE340C02C03645697UL,
                0x2FDE0B3CAB765133UL,0x66B6DA474337E6ECUL,0x7A93EAE5159CC7D3UL,0xF00E3A46C0E289F2UL,
                0x808B670962AA60D8UL,0xE9F89CBD09A88BFUL,0xB0B183A1BDDCC26EUL,0xF634C25418FF057FUL,
                0x2EA1BD83598B2103UL,0x333902DAD0697D3BUL,0xC4367B37E9EB5874UL,0x85A1261BEE9532BDUL,
                0x688AC5E17DEAD255UL,0xDF97D007F1A02304UL,0x682B3B1148504FBAUL,0xC06DAC08891710B9UL,
                0x4F21B2CB336E5836UL,0xCAD5369152DCF25AUL,0x1D6A128716CE2DF0UL,0xF69C13D35CCD28E5UL,
                0xEBA882E23D576927UL,0xD7DB47C44F7792F5UL,0x918F9AEE4CD043DBUL,0x6460E6F26A6183DDUL,
                0x205EED63CA741408UL,0x3839391F75858B24UL,0x1AC435EF3418B34CUL,0x519F146C5C7F01E6UL,
                0xF25560E0A1B19882UL,0x6E4C166F694815B2UL,0x949A9D32C8569BCFUL,0x546343CF3384D32EUL,
                0x3BCBCBBA989FECDEUL,0x5728F4440F828304UL,0x44F5732354F0D15CUL,0x1E9A26B751A7E64EUL,
                0xD1494C05BC23FC3FUL,0x22851434E6C10B29UL,0x8C955C37F9108776UL,0xD1E3E16AF0803058UL,
                0x6F153A25BA371E8EUL,0x4DCF068EA512D7D7UL,0xABAE5C2B0982F439UL,0xC51C736AF5A88AEUL,
                0x23188E7C4D4D9D83UL,0xF62C3D5F42712323UL,0x1C6B7E6F95C99D37UL,0x5767AF5596E9ED42UL,
                0xFC00E2D72821B4DFUL,0x7C2A581BE4E56913UL,0x9CBC14B305D84EF6UL,0x5B097AB62796EA29UL,
                0xD2789AD5A20E2A5BUL,0xA50A8F8CCE28004EUL,0xEB2BD0DCCEA4047BUL,0xB0916572F76F8ED7UL,
                0x58C09A536ADF77FEUL,0x34D4042644062D8AUL,0x5AF7D0E5F6DAF7E5UL,0x5488D44F51FA7FBEUL,
                0xFB198BBFB8CEDEA6UL,0xC43B3715F118B27CUL,0x52FE5699756CB000UL,0x44AFF50C0A09AC96UL,
                0x7D20FA639F5C9710UL,0xE8097BF8DD226223UL,0xF06552C752A8EAFAUL,0x66A0555FFE8B7088UL,
                0xA1CB180245840BAUL,0xC6F8C2631272B1CCUL,0x6CAC8AA3EA7E7D58UL,0xEF063BAC59E54D48UL,
                0xEEF9330806DCD00AUL,0x61C2B3150B9E1F20UL,0x4CE4E45BA3A73D59UL,0x7074AA6FFF59ACACUL,
                0x26468FB1A970E459UL,0x4F4F7C9EC9D300F5UL,0x76EB07BD6C6F01BFUL,0x32B7D34E963B3EF0UL,
                0xC500F659B31D3F8EUL,0x803E450A07515A5CUL,0xBEED4DC90AACB192UL,0xCF2952FD682F8D9BUL,
                0x68CCED0530E23CD7UL,0xA5D0A2844A1E6BF1UL,0x1C1C9ECEE76849UL,0xD84DBF46B11768CDUL,
                0xF36CF91F6D8CD06UL,0xAE023ED346E9B9A9UL,0xFFB93AC30E2BB839UL,0xCB235E2D4F42F24UL,
                0x500CCBF60CF13AAAUL,0xE029CD7B202BBF4BUL,0x5D9FD12E295975A4UL,0xB5085734E8BE7FC5UL,
                0x3578AAC7D044A2D6UL,0xE47D3E4F3C532D91UL,0xF7189E706E69C464UL,0x4CACD0F174DD0F55UL,
                0x89184015CA1B18F4UL,0x4CEF388B480C0943UL,0x21228726358414AAUL,0xBA2464E525757706UL,
                0xA7A650BD48771B4EUL,0x30BE9D6E2979F9E5UL,0x47263C9D35B03328UL,0xFF061B5353D15473UL,
                0x5594D0D9E36A2A2BUL,0x9B512AD7689FED28UL,0xE78E37951963FBD8UL,0x7A3FED19F7F6D4A0UL,
                0x90350861CB4ADFC9UL,0xF3D9CFCC85E8C2C5UL,0x7B4197C5EA43E3E7UL,0xE930336270107C33UL,
                0xE7D5F42774D26EF7UL,0x8B889158679D94F3UL,0xF594A00F70608759UL,0x6CCA64B2FFC79E4AUL,
                0xD6230B6F565EEEF0UL,0x53B071120120C51UL,0xCCDC12A4917D0319UL,0x269B2F10219283D7UL,
                0x17EDE354FC315EA0UL,0xEF8C91363822CBF0UL,0x6AA328AB3E6E43D6UL,0x195266525FA2157EUL,
                0x6E5692B6152A69E5UL,0x747A91F26D08C736UL,0xEFB8278A9E60ACE5UL,0xC64EB8292A9E304CUL,
                0xB8700A01A2F8A97UL,0x1F2BD116C8621178UL,0xCF4C3704D135A45AUL,0x54AE33A712D0FD18UL,
                0xA1D9CE4476D8ED0DUL,0xA4CF34AF98ECB89DUL,0xC7D78C5FAF1073BCUL,0xDD5BE87374D931A0UL,
                0x6DCF7A3D704640E1UL,0xECCF4DFCD3EBF61FUL,0xCEBB6F86DE4456DBUL,0x4B836346CF1742BAUL,
                0x8CED3DFDCCF75D7EUL,0xF43075220CF738C1UL,0xC972F3D701596158UL,0x395746102A5F0C97UL,
                0xB4E614002A1D78DCUL,0x4958989E7ED5E569UL,0x2419D1C9D70C683UL,0x236C129CF0C14AC3UL,
                0x113A90CCE809B304UL,0x71A24DDC56864BDFUL,0xACA7CBFB1AB40A2EUL,0x36456978ED4C45AUL,
                0xAB76513357D46931UL,0x4337E6ECD263BAF3UL,0x159CC7D3070ED069UL,0xC0E289F2CD073AA4UL,
                0x62AA60D8E9585305UL,0xD09A88BF5C37E342UL,0xBDDCC26E995D5FD3UL,0x18FF057F228317C4UL,
                0x598B21034B25F6C8UL,0xD0697D3BF2F56413UL,0xE9EB587487D92AF7UL,0xEE9532BDF49F1019UL,
                0x7DEAD25575E12964UL,0xF1A02304A828813AUL,0x48504FBA6549020BUL,0x891710B9E4B856EFUL,
                0x336E583603B01726UL,0x52DCF25AFF2A1920UL,0x16CE2DF01D4CB20FUL,0x5CCD28E568C0B1A8UL,
                0x3D5769273D69C50BUL,0x4F7792F5C8239347UL,0x4CD043DBA70A1621UL,0x6A6183DDA3C40041UL,
                0xCA7414080C4C0F64UL,0x75858B244987B5E5UL,0x3418B34C949EAEC4UL,0x5C7F01E6FE543631UL,
                0xA1B198826CFA4D0CUL,0x694815B2B32ED728UL,0xC8569BCF8305EEEFUL,0x3384D32E81720706UL,
                0x989FECDED1483652UL,0xF828304958EB4CBUL,0x54F0D15C953EED61UL,0x51A7E64EE865C807UL,
                0xBC23FC3FF636BBA2UL,0xE6C10B299F3737DAUL,0xF910877690C62329UL,0xF08030588B0B3B8EUL,
                0xBA371E8E45647013UL,0xA512D7D710CE3C89UL,0x982F439EC64BAFCUL,0xAF5A88AEF7DE6989UL,
                0x4D4D9D83C54A7DDAUL,0x42712323227F038CUL,0x95C99D37E7C0752CUL,0x96E9ED424847C743UL,
                0x2821B4DF97C47D94UL,0xE4E5691394320EFFUL,0x5D84EF6B6490D84UL,0x2796EA29D32F7F28UL,
                0xA20E2A5B2CC43252UL,0xCE28004E46949FB5UL,0xCEA4047B8A8EA34BUL,0xF76F8ED7FBC1585BUL,
                0x6ADF77FED0970B17UL,0x44062D8AA922754AUL,0xF6DAF7E5187140C3UL,0x51FA7FBEFAA34409UL,
                0xB8CEDEA638F0A341UL,0xF118B27CAB931E0CUL,0x756CB000B123E3E3UL,0xA09AC96F9694C87UL,
                0x9F5C9710A9FDF73AUL,0xDD226223DA616C22UL,0x52A8EAFA12541F3DUL,0xFE8B708803208500UL,
                0x245840BA25E1CC36UL,0x1272B1CC4B28B1CBUL,0xEA7E7D58909A0F4EUL,0x59E54D48B7CF3B06UL,
                0x6DCD00A9AF9DE78UL,0xB9E1F200C20A3F0UL,0xA3A73D59692AAFB6UL,0xFF59ACAC0FC4C356UL,
                0xA970E459DD425787UL,0xC9D300F5FF6111A5UL,0x6C6F01BF01E7CAD3UL,0x963B3EF0C2B4225DUL,
                0xB31D3F8E3A6A29F4UL,0x7515A5C555392DDUL,0xAACB1921E7962CEUL,0x682F8D9B3605BC86UL,
                0x30E23CD74E0624B0UL,0x4A1E6BF1545B05C3UL,0xCEE7684924374451UL,0xB11768CDBCEE6B6FUL,
                0xF6D8CD06B921B2AFUL
        };


        protected static int GetZKeyIndex(PieceType type,int rank,int file,Side owner)
        {
            int index = 0;
            switch (type)
            { 
                case PieceType.Pawn:
                    index = 0;
                    break;
                case PieceType.Knight:
                    index = 2;
                    break;
                case PieceType.Bishop:
                    index = 4;
                    break;
                case PieceType.Rook:
                    index = 6;
                    break;
                case PieceType.Queen:
                    index = 8;
                    break;
                case PieceType.King:
                    index = 10;
                    break;
            }
            if (owner == Side.White)
                index++;
            return 64 * index + 8 * rank + file;
        }

        static Piece()
        {
            GenerateKingAttacksLookup();
            GenerateLeaperAttacksLookup();
            GenerateStraightAttacksLookup();
            GenerateDiagAttacksLookup();
        }

        protected static bool[] KingAttackLookup = new bool[240];
        protected static int[] DiagAttackLookup = new int[240];
        protected static int[] StraightAttackLookup = new int[240];
        protected static bool[] LeaperAttackLookup = new bool[240];
        static HashSet<int> destCells = new HashSet<int>();
        static void MakeDict(int n, int[] moves)
        {
            destCells.Clear();
            for (int q = 0; q < n; ++q)
            {
                destCells.Add(MovePackHelper.GetEndSquare(moves[q]));
            }
        }
        private static void GenerateKingAttacksLookup()
        {
            int[] probe=new int[8];
            for (int i = 0; i <= 119; ++i)
            {
                King k = new King(Side.White,new OX88Chessboard("/8/8/8/8/8/8/8/8 w - - 0 1"), i);
                int n = k.GetMoves(0, probe);
                MakeDict(n, probe);
                for (int j = 0; j <= 119; ++j)
                {
                    if (Square.SquareValid(i) && Square.SquareValid(j))
                    {
                        if (destCells.Contains(j))
                            KingAttackLookup[Square.Ox88Dist(i, j)] = true;
                        
                    }
                }
            }
        }
        private static void GenerateLeaperAttacksLookup()
        {
            int[] probe = new int[8];
            for (int i = 0; i <= 119; ++i)
            {
                Knight k = new Knight(Side.White, new OX88Chessboard("/8/8/8/8/8/8/8/8 w - - 0 1"), i);
                int n = k.GetMoves(0, probe);
                MakeDict(n, probe);
                for (int j = 0; j <= 119; ++j)
                {
                    if (Square.SquareValid(i) && Square.SquareValid(j))
                    {
                        if (destCells.Contains(j))
                            LeaperAttackLookup[Square.Ox88Dist(i, j)] = true;
                    }
                }
            }
        }
        private static void GenerateStraightAttacksLookup()
        {
            int[] probe = new int[14];
            for (int i = 0; i <= 119; ++i)
            {
                Rook k = new Rook(Side.White, new OX88Chessboard("/8/8/8/8/8/8/8/8 w - - 0 1"), i);
                int n = k.GetMoves(0, probe);
                MakeDict(n, probe);
                for (int j = 0; j <= 119; ++j)
                {
                    if (Square.SquareValid(i) && Square.SquareValid(j))
                    {
                        if (destCells.Contains(j))
                            StraightAttackLookup[Square.Ox88Dist(i, j)] = GetStraightAttackDirFrom(i, j); 
                    }
                }
            }
        }
        private static void GenerateDiagAttacksLookup()
        {
            int[] probe = new int[14];
            for (int i = 0; i <= 119; ++i)
            {
                Bishop k = new Bishop(Side.White, new OX88Chessboard("/8/8/8/8/8/8/8/8 w - - 0 1"), i);
                int n = k.GetMoves(0, probe);
                MakeDict(n, probe);
                for (int j = 0; j <= 119; ++j)
                {
                    if (Square.SquareValid(i) && Square.SquareValid(j))
                    {
                        if (destCells.Contains(j))
                            DiagAttackLookup[Square.Ox88Dist(i, j)] = GetDiagAttackDirFrom(i, j); 
                    }
                }
            }
        }

        private static int GetDiagAttackDirFrom(int i, int j)
        {
            if (Square.Rank(i) > Square.Rank(j))
            {
                if (Square.Col(i) > Square.Col(j))
                    return SW;
                else
                    return SE;
            }
            if (Square.Rank(i) < Square.Rank(j))
            {
                if (Square.Col(i) > Square.Col(j))
                    return NW;
                else
                    return NE;
            }
            throw new Exception("Internal Error");
        }

        private static int GetStraightAttackDirFrom(int i, int j)
        {
            if (Square.Rank(i) == Square.Rank(j))
            {
                if( Square.Col(i) > Square.Col(j) )
                    return WEST;
                else
                    return EAST;
            }
            if (Square.Col(i) == Square.Col(j))
            {
                if (Square.Rank(i) > Square.Rank(j))
                    return SOUTH;
                else
                    return NORTH;
            }
            throw new Exception("Internal Error");

        }

        protected Side owner;
        protected IChessBoard board;
        public PinStatus PinStatus { get; set; }

        public abstract bool Diagonal { get; }
        public abstract bool Straight { get; }
        public abstract PieceType Type { get; }
        public int CapValue
        {
            get { return GetCapValue(); }
        }
        protected bool AttackPathFree(int dir,int square)
        {
            int sq = HomeSquare;
            sq -= dir;
            while (sq != square)
            {
                if (board.BoardArray[sq] != null)
                    return false;
                sq -= dir;
            }
            return true;
        }

        protected bool PinCompatible(int dir)
        {
            if (PinStatus == PinStatus.None)
                return true;
            if ((dir == NORTH || dir == SOUTH) && PinStatus == PinStatus.NS)
                return true;
            if ((dir == SE || dir == NW) && PinStatus == PinStatus.NWSE)
                return true;
            if ((dir == SW || dir == NE) && PinStatus == PinStatus.SWNE)
                return true;
            return false;
        }
        protected bool IsEnemy(int square)
        {
            return board.BoardArray[square] != null && board.BoardArray[square].Owner != Owner;
        }
        public Piece(Side owner,IChessBoard board,int cell)
        {
            this.owner = owner;
            this.board = board;
            this.HomeSquare = cell;
        }
        protected abstract int GetValue();
        protected abstract int GetCapValue();
        public abstract bool AttackSquare(int square);
        public int Value
        {
            get
            {
                return GetValue();
            }
            
        }
        public Side Owner
        {
            get
            {
                return owner;
            }

        }
        
        public int CompareTo(object obj)
        {
            return (obj as Piece).Value - Value;
        }
        public int HomeSquare
        {
            get ;  set;
        }

        

        public void Move(int move)
        {
            OnMove(move);
            board.BoardArray[HomeSquare] = null;
            board.BoardArray[MovePackHelper.GetEndSquare(move)] = this;
            board.ZKey ^= ZKey;
            HomeSquare = MovePackHelper.GetEndSquare(move);
            board.ZKey ^= ZKey;
        }
        public IChessBoard Board
        {
            get { return board; }
        }
        public void UnMove(int move)
        {
            board.BoardArray[HomeSquare] = null;
            board.BoardArray[MovePackHelper.GetStartSquare(move)] = this;
            board.ZKey ^= ZKey;
            HomeSquare = MovePackHelper.GetStartSquare(move);
            board.ZKey ^= ZKey;
            OnUnMove(move);
        }
        public void UnCapture(int square)
        {
            HomeSquare = square;
            board.ZKey ^= ZKey;
        }
        public void Capture()
        {
            board.ZKey ^= ZKey;
            HomeSquare = Square.Invalid;
        }
        public void ForceSquare(int square)
        {
            HomeSquare = square;
        }
        public int GetMoves(int start, int[] moves)
        {
            return OnGetMoves(start,moves);
        }
        public int GetCaptureMoves(int start, int[] moves,int square)
        {
            return OnGetCaptureMoves(start, moves,square );
        }
        public int GetBlockingMoves(int start, int[] moves, int[] ray, int rayLen)
        {
            return OnGetBlockingMoves(start, moves, ray, rayLen);
        }


        public const int Infinite = 1000000;
        
        public override string ToString()
        {
            return GetPieceString();
        }
        protected abstract string GetPieceString();
        protected abstract void OnMove(int move);
        protected abstract void OnUnMove(int move);
        protected abstract int OnGetMoves(int start, int[] moves);
        protected abstract int OnGetCaptureMoves(int start, int[] moves, int square);
        protected abstract int OnGetBlockingMoves(int start, int[] moves, int[] ray, int rayLen);
    }
}
