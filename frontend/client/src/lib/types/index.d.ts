type Activity = {
    id: string
    title: string
    date: string
    description: string
    category: string
    isCancelled: boolean
    hostId: string
    hostDisplayName: string
    city: string
    venue: string
    latitude: number
    longitude: number
    attendees: Profile[]
}

type Profile = {
    id: string
    displayName: string
    bio?: string
    imageUrl?: string
}

type User = {
    id: string
    email: string
    displayName: string
    imageUrl?: string
}