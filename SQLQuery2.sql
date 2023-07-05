INSERT INTO Subscription ( SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime)
                        OUTPUT INSERTED.ID
                        VALUES ( 2, 1, 07/03/2023, null );

select SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime from Subscription