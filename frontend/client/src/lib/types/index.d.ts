type Activity = {
  id: string;
  title: string;
  date: string;
  description: string;
  category: string;
  isCancelled: boolean;
  hostId: string;
  hostDisplayName: string;
  city: string;
  venue: string;
  latitude: number;
  longitude: number;
  price: number;
  hostId: string;
  hostDisplayName: string;
  attendees: Profile[];
};

type Profile = {
  id: string;
  displayName: string;
  bio?: string;
  imageUrl?: string;
};

type User = {
  id: string;
  email: string;
  displayName: string;
  imageUrl?: string;
  token: string;
};


//type PrepaidAccount = {
 // balance: number;
 // transactions: AccountTransaction[];
// };

type PrepaidAccount = {
  id: string;
  balance: number;
};

/*type AccountTransaction = {
  id: string;
  type: string;
  amount: number;
  balanceBefore: number;
  balanceAfter: number;
  activityId?: string;
  activityTitle?: string;
  createdAt: string;
}; */

type AccountTransaction = {
  id: string;
  type: "Debit" | "Refund" | "Credit";
  amount: number;
  balanceBefore: number;
  balanceAfter: number;
  activityId?: string;
  createdAt: string;
};